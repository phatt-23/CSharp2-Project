namespace CoworkingApp.Services;

public static class Pagination
{
    static public IQueryable<T> Paginate<T>(IQueryable<T> collection, int pageNumber = 1, int pageSize = 10)
    {
        return Paginate(collection.AsEnumerable(), pageNumber, pageSize).AsQueryable();
    }
    
    static public IEnumerable<T> Paginate<T>(IEnumerable<T> collection, int pageNumber = 1, int pageSize = 10)
    {
        if (pageNumber < 1 || pageSize < 1)
            throw new InvalidOperationException("PageNumber and PageSize must be greater than or equal to 1");
        
        var page = collection.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        return page;
    }
}