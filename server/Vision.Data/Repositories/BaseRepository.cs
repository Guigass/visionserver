using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq.Expressions;
using Vision.Data.Context;
using Vision.Data.Interfaces;
using Vision.Data.Models;

namespace Vision.Data.Repositories;
public abstract class BaseRepository<TEntity> : IRepositoryBase<TEntity> where TEntity : Entity
{
    protected readonly VisionContext _context;
    public DbSet<TEntity> DbSet => _context.Set<TEntity>();

    public BaseRepository(VisionContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> expression)
    {
        return await DbSet.AsNoTracking().Where(expression).ToListAsync();
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await DbSet.ToListAsync();
    }

    public async Task<TEntity> GetByIdAsync(Guid id)
    {
        var entity = await DbSet.FindAsync(id);

        if (entity == null)
            return null!;

        return entity;
    }

    public async Task CreateAsync(TEntity entity)
    {
        await DbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(TEntity entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            DbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteAsync(TEntity entity, string[]? includes = null)
    {
        var query = DbSet.AsNoTracking();

        if (includes != null)
            foreach (var include in includes)
                query = query.Include(include);

        DbSet.Remove(entity);

        await _context.SaveChangesAsync();
    }

    public async Task SaveChanges()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<TEntity>> ListAsync(ISpecification<TEntity> spec)
    {
        return await ApplySpecification(spec).ToListAsync();
    }

    public async Task PartialUpdateAsync(TEntity originalEntity, TEntity updatedEntity)
    {
        foreach (var property in updatedEntity.GetType().GetProperties())
        {
            var oldValue = originalEntity.GetType().GetProperty(property.Name)!.GetValue(originalEntity, null);
            var newValue = property.GetValue(updatedEntity, null);

            if (newValue != null || oldValue != null)
            {
                if ((newValue == null && oldValue != null) ||
                    (newValue != null && oldValue == null) ||
                    !newValue!.Equals(oldValue))
                {
                    property.SetValue(updatedEntity, newValue);
                }
            }
        }

        _context.Entry(originalEntity).CurrentValues.SetValues(updatedEntity);

        await SaveChanges();
    }

    public async Task<int> CountAsync(ISpecification<TEntity> spec)
    {
        return await ApplySpecification(spec).CountAsync();
    }

    public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predictate)
    {
        var query = DbSet.AsNoTracking();

        return await query.Where(predictate).CountAsync();
    }

    public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predictate, string[]? includes = null)
    {
        var query = DbSet.AsNoTracking();

        if (includes != null)
        {
            foreach (var include in includes)
                query = query.Include(include);
        }

        return await query.Where(predictate).CountAsync();
    }

    public async Task<decimal> SumAsync(ISpecification<TEntity> spec, Expression<Func<TEntity, decimal>> seletor)
    {
        return await ApplySpecification(spec).SumAsync(seletor);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    private IQueryable<TEntity> ApplySpecification(ISpecification<TEntity> spec)
    {
        return SpecificationEvaluator<TEntity>.GetQuery(DbSet.AsQueryable(), spec);
    }

    public async Task<IEnumerable<TEntity>> SearchAsync(Expression<Func<TEntity, bool>> predictate, string[]? includes = null)
    {
        var query = DbSet.AsNoTracking();

        if (includes != null)
        {
            foreach (var include in includes)
                query = query.Include(include);
        }

        return await query.Where(predictate).ToListAsync();
    }

    public async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predictate, string[]? includes = null)
    {
        var query = DbSet.AsNoTracking();

        if (includes != null)
        {
            foreach (var include in includes)
                query = query.Include(include);
        }

        return await query.FirstOrDefaultAsync(predictate);
    }

    public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predictate, string[]? includes = null)
    {
        var query = DbSet.AsNoTracking();

        if (includes != null)
        {
            foreach (var include in includes)
                query = query.Include(include);
        }

        return await query.AnyAsync(predictate);
    }

    public async Task<IEnumerable<IGrouping<IKey, TEntity>>> GroupByAsync<Key>(Expression<Func<TEntity, IKey>> keySelector, Expression<Func<TEntity, bool>> predicate, string[] includes = null)
    {
        var query = _context.Set<TEntity>().AsQueryable();

        if (includes != null)
        {
            query = includes.Aggregate(query, (current, include) => current.Include(include));
        }

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        return await query.GroupBy(keySelector).ToListAsync();
    }

    public void Dispose()
    {
        _context?.Dispose();

        GC.SuppressFinalize(this);
    }
}
