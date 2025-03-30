using Microsoft.EntityFrameworkCore;

namespace CoworkingApp.Services;


public interface IPaginationService
{
    IQueryable<T> Paginate<T>(IQueryable<T> collection, int pageNumber, int pageSize);
    IEnumerable<T> Paginate<T>(IEnumerable<T> collection, int pageNumber, int pageSize);
}


public class PaginationService : IPaginationService
{
    public IQueryable<T> Paginate<T>(IQueryable<T> collection, int pageNumber = 1, int pageSize = 10)
    {
        return Paginate(collection.AsEnumerable(), pageNumber, pageSize).AsQueryable();
    }
    
    public IEnumerable<T> Paginate<T>(IEnumerable<T> collection, int pageNumber = 1, int pageSize = 10)
    {
        if (pageNumber < 1 || pageSize < 1)
            throw new InvalidOperationException("PageNumber and PageSize must be greater than or equal to 1");
        
        var page = collection.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        return page;
    }
}