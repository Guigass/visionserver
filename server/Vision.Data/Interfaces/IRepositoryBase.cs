using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Vision.Data.Interfaces;
public interface IRepositoryBase<TEntity> : IDisposable where TEntity : class
{
    Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> expression);
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<TEntity> GetByIdAsync(Guid id);
    Task<IEnumerable<TEntity>> ListAsync(ISpecification<TEntity> spec);
    Task CreateAsync(TEntity entity);
    Task UpdateAsync(TEntity entity);
    Task PartialUpdateAsync(TEntity originalEntity, TEntity updatedEntity);
    Task DeleteAsync(Guid id);
    Task DeleteAsync(TEntity entity, string[] includes = null!);
    Task<IEnumerable<TEntity>> SearchAsync(Expression<Func<TEntity, bool>> predictate, string[] includes = null!);
    Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predictate, string[] includes = null!);
    Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predictate, string[]? includes = null);
    Task<int> CountAsync(ISpecification<TEntity> spec);
    Task<int> CountAsync(Expression<Func<TEntity, bool>> predictate);
    Task<int> CountAsync(Expression<Func<TEntity, bool>> predictate, string[]? includes = null);
    Task<decimal> SumAsync(ISpecification<TEntity> spec, Expression<Func<TEntity, decimal>> seletor);
    Task<IEnumerable<IGrouping<IKey, TEntity>>> GroupByAsync<Key>(Expression<Func<TEntity, IKey>> keySelector, Expression<Func<TEntity, bool>> predicate, string[] includes = null);
    Task<int> SaveChangesAsync();
}
