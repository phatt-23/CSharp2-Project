using CoworkingApp.Data;
using CoworkingApp.Models.DataModels;
using Microsoft.EntityFrameworkCore;

namespace CoworkingApp.Services;

public interface ICoworkingCenterRepository
{
    Task<IEnumerable<CoworkingCenter>> GetCoworkingCentersAsync(CoworkingCenterFilterOptions options);
    Task<CoworkingCenter> AddCoworkingCenterAsync(CoworkingCenter coworkingCenter);
}

public class CoworkingCenterRepository(CoworkingDbContext context) : ICoworkingCenterRepository
{
    public Task<IEnumerable<CoworkingCenter>> GetCoworkingCentersAsync(CoworkingCenterFilterOptions options)
    {
        var ccs = context.CoworkingCenters
            .Where(c => options.Id == null || options.Id == c.Id)
            .Where(c => options.LikeName == null || c.Name.Contains(options.LikeName))
            .Where(c => options.LatitudeLow == null || options.LatitudeLow <= c.Latitude)
            .Where(c => options.LatitudeHigh == null || options.LatitudeHigh >= c.Latitude)
            .Where(c => options.CreatedAtStart == null || options.CreatedAtStart >= c.CreatedAt)
            .Where(c => options.CreatedAtEnd == null || options.CreatedAtEnd <= c.CreatedAt);
        
        if (options.IncludeWorkspaces)
            ccs = ccs.Include(cc => cc.Workspaces);

        return Task.FromResult<IEnumerable<CoworkingCenter>>(ccs);
    }

    public async Task<CoworkingCenter> AddCoworkingCenterAsync(CoworkingCenter coworkingCenter)
    {
        if (coworkingCenter.Latitude is < -90 or > 90)
            throw new ArgumentException("Latitude must be between -90 and 90.");
        
        if (coworkingCenter.Longitude is < 0 or > 180)
            throw new ArgumentException("Longitude must be between -90 and 90.");

        var cc = await context.CoworkingCenters.AddAsync(coworkingCenter);
        await context.SaveChangesAsync();
        return cc.Entity;
    }
}

public class CoworkingCenterFilterOptions
{
     public int? Id { get; set; }
     public string? LikeName { get; set; }
     public string? LikeDescription { get; set; }
     public decimal? LatitudeLow { get; set; }
     public decimal? LatitudeHigh { get; set; }
     public decimal? LongitudeLow { get; set; }
     public decimal? LongitudeHigh { get; set; }
     public DateTime? CreatedAtStart { get; set; }   
     public DateTime? CreatedAtEnd { get; set; }
     public bool IncludeWorkspaces { get; set; } = false;
}