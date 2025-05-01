using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace CoworkingApp.Services;


// Result of pagination
public class PaginationResult<T>
{
    public required int TotalCount { get; set; }
    public required ICollection<T> Page { get; set; }
}

public static class Pagination
{
    public static IQueryable<T> Paginate<T>(
        this IQueryable<T> query,
        out int totalCount,
        int pageNumber = 1,
        int pageSize = 10)
    {
        if (pageNumber < 1 || pageSize < 1)
            throw new InvalidOperationException("PageNumber and PageSize must be greater than or equal to 1");
        totalCount = query.Count();
        return query
               .Skip((pageNumber - 1) * pageSize)
               .Take(pageSize);
    }

    public static IEnumerable<T> Paginate<T>(
        IEnumerable<T> collection, 
        out int totalCount,
        int pageNumber = 1, 
        int pageSize = 10)
    {
        var query = collection.AsQueryable();
        if (pageNumber < 1 || pageSize < 1)
        {
            throw new InvalidOperationException("PageNumber and PageSize must be greater than or equal to 1");
        }

        totalCount = query.Count();
        return collection.Skip((pageNumber - 1) * pageSize).Take(pageSize);
    }
}