using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoworkingApp.Services;

public abstract class RepositoryBase<TEntity, TFilter>
    (
        DbContext context
    )
    where TEntity : class
{
    public virtual IQueryable<TEntity> ApplyFilter(IQueryable<TEntity> query, TFilter filter) => query;

    public async Task<TEntity> AddAsync(TEntity entity)
    {
        var entry = await context.Set<TEntity>().AddAsync(entity);
        await context.SaveChangesAsync();
        return entry.Entity;
    }
}


public class RangeFilter<T> where T : struct, IComparable<T>
{
    [FromQuery(Name = "min")]
    public T? Min { get; set; }

    [FromQuery(Name = "max")]
    public T? Max { get; set; }
    
    public IQueryable<TEntity> ApplyTo<TEntity>(
        IQueryable<TEntity> query, 
        Expression<Func<TEntity, T>> propertySelector)
    {
        var selectorFunc = propertySelector.Compile();
        var compare = Comparer<T>.Default.Compare;
        
        IEnumerable<TEntity> list = query.AsEnumerable();

        if (Min.HasValue)
            list = list.Where(x => compare(selectorFunc(x), Min.Value) >= 0);

        if (Max.HasValue)
            list = list.Where(x => compare(selectorFunc(x), Max.Value) <= 0);

        return list.AsQueryable();
    }
}


public class NullableRangeFilter<T> where T : struct, IComparable<T>
{
    [FromQuery(Name = "min")]
    public T? Min { get; set; }

    [FromQuery(Name = "max")]
    public T? Max { get; set; }

    public IQueryable<TEntity> ApplyTo<TEntity>(
        IQueryable<TEntity> query, 
        Expression<Func<TEntity, T?>> propertySelector)
    {
        var selectorFunc = propertySelector.Compile();
        var compare = Comparer<T>.Default.Compare;
        
        IEnumerable<TEntity> list = query.AsEnumerable();

        if (Min.HasValue)
            list = list.Where(x => selectorFunc(x) == null || compare(selectorFunc(x)!.Value, Min.Value) >= 0);

        if (Max.HasValue)
            list = list.Where(x => selectorFunc(x) == null || compare(selectorFunc(x)!.Value, Max.Value) <= 0);

        return list.AsQueryable();
    }
}

