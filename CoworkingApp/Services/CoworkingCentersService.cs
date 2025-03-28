using CoworkingApp.Data;
using CoworkingApp.Models.DataModels;
using CoworkingApp.Models.DTOModels.CoworkingCenters;
using Microsoft.EntityFrameworkCore;

namespace CoworkingApp.Services;

public class CoworkingCentersService(CoworkingDbContext dbContext)
{
    private readonly CoworkingDbContext _context = dbContext;
    
    public async Task<(ICollection<CoworkingCenter>, int)> GetAsync(CoworkingCentersQueryDto query)
    {
        var coworkingCenters = _context.CoworkingCenters.AsQueryable();

        if (query.Id.HasValue)
        {
            coworkingCenters = coworkingCenters.Where(c => c.Id == query.Id.Value);
        }

        if (query.Name != null)
        {
            coworkingCenters = coworkingCenters.Where(c => c.Name == query.Name);
        }

        coworkingCenters = coworkingCenters
            .Include(cc => cc.Workspaces);

        var totalCount = await coworkingCenters.CountAsync();
       
        // TODO: Add pagination
        
        return (await coworkingCenters.ToListAsync(), totalCount);
    }
}