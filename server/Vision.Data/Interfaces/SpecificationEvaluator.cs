using Microsoft.EntityFrameworkCore;

namespace Vision.Data.Interfaces;
public class SpecificationEvaluator<T> where T : class
{
    public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecification<T> specification)
    {
        var query = inputQuery;

        // modify the IQueryable using the specification's criteria expression
        if (specification.Criteria != null)
        {
            query = query.Where(specification.Criteria);
        }

        // Includes all expression-based includes
        query = specification.Includes.Aggregate(query,
            (current, include) => current.Include(include));

        // Include any string-based include statements
        query = specification.IncludeStrings.Aggregate(query,
            (current, include) => current.Include(include));

        // Apply ordering if expressions are set
        if (specification.OrderBy != null)
        {
            if (specification.ThenBy != null)
            {
                query = query.OrderBy(specification.OrderBy)
                             .ThenBy(specification.ThenBy);
            }
            else if (specification.ThenByDescending != null)
            {
                query = query.OrderBy(specification.OrderBy)
                             .ThenByDescending(specification.ThenByDescending);
            }
            else
                query = query.OrderBy(specification.OrderBy);
        }
        else if (specification.OrderByDescending != null)
        {
            if (specification.ThenBy != null)
            {
                query = query.OrderByDescending(specification.OrderByDescending)
                             .ThenBy(specification.ThenBy);
            }
            else if (specification.ThenByDescending != null)
            {
                query = query.OrderByDescending(specification.OrderByDescending)
                             .ThenByDescending(specification.ThenByDescending);
            }
            else
                query = query.OrderByDescending(specification.OrderByDescending);
        }

        // Apply paging if enabled
        if (specification.IsPagingEnabled)
        {
            query = query.Skip(specification.Skip)
                .Take(specification.Take);
        }
        return query;
    }
}
