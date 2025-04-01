using AutoFilterer.Attributes;
using AutoFilterer.Enums;
using AutoFilterer.Extensions;
using AutoFilterer.Types;
using CoworkingApp.Data;
using CoworkingApp.Models.DataModels;
using Microsoft.EntityFrameworkCore;

namespace CoworkingApp.Services;

public interface ICoworkingCenterRepository
{
    Task<IEnumerable<CoworkingCenter>> GetCoworkingCentersAsync(CoworkingCenterFilter options);
    Task<CoworkingCenter> AddCoworkingCenterAsync(CoworkingCenter coworkingCenter);
}

public class CoworkingCenterRepository(CoworkingDbContext context) : ICoworkingCenterRepository
{
    public Task<IEnumerable<CoworkingCenter>> GetCoworkingCentersAsync(CoworkingCenterFilter filter)
    {
        var query = context.CoworkingCenters.ApplyFilter(filter);
            
        query = filter.Latitude.ApplyTo(query, c => c.Latitude);
        query = filter.Longitude.ApplyTo(query, c => c.Latitude);
        query = filter.CreatedAt.ApplyTo(query, c => c.CreatedAt);
        
        if (filter.IncludeWorkspaces)
            query = query.Include(cc => cc.Workspaces);

        return Task.FromResult<IEnumerable<CoworkingCenter>>(query);
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

public class CoworkingCenterFilter : FilterBase
{
     [CompareTo(nameof(CoworkingCenter.Id))]
     public int? Id { get; set; }

     [StringFilterOptions(StringFilterOption.Contains)]
     [CompareTo(nameof(CoworkingCenter.Name))]
     public string? LikeName { get; set; }

     [StringFilterOptions(StringFilterOption.Contains)]
     [CompareTo(nameof(CoworkingCenter.Description))]
     public string? LikeDescription { get; set; }
     
     public RangeFilter<decimal> Latitude { get; set; } = new();
     public RangeFilter<decimal> Longitude { get; set; } = new();
     public RangeFilter<DateTime> CreatedAt { get; set; } = new();
     
     public bool IncludeWorkspaces { get; set; } = false;
}

