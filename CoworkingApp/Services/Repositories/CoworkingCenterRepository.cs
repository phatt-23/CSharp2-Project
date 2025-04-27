using System.Runtime.InteropServices;
using AutoFilterer.Attributes;
using AutoFilterer.Enums;
using AutoFilterer.Extensions;
using AutoFilterer.Types;
using CoworkingApp.Data;
using CoworkingApp.Models.DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.EnvironmentVariables;

namespace CoworkingApp.Services.Repositories;

public interface ICoworkingCenterRepository
{
    Task<CoworkingCenter> AddCenter(CoworkingCenter coworkingCenter);
    Task<IEnumerable<CoworkingCenter>> GetCenters(CoworkingCenterFilter options);
    Task<CoworkingCenter> GetCenterById(int id);
    Task<CoworkingCenter> UpdateCenter(CoworkingCenter coworkingCenter);
}

public class CoworkingCenterRepository
    (
        CoworkingDbContext context
    ) 
    : ICoworkingCenterRepository
{
    public Task<IEnumerable<CoworkingCenter>> GetCenters(CoworkingCenterFilter filter)
    {
        var query = context.CoworkingCenters.ApplyFilter(filter);
            
        query = filter.Latitude.ApplyTo(query, c => c.Address.Latitude);
        query = filter.Longitude.ApplyTo(query, c => c.Address.Latitude);
        
        if (filter.IncludeWorkspaces)
            query = query.Include(cc => cc.Workspaces);

        if (filter.IncludeAddress)
        {
            query = query.Include(cc => cc.Address)
                .Include(cc => cc.Address.City)
                .Include(cc => cc.Address.City.Country);
        }

        return Task.FromResult<IEnumerable<CoworkingCenter>>(query);
    }

    public async Task<CoworkingCenter> GetCenterById(int id)
    {
        var ccs = await GetCenters(new () { Id = id, IncludeWorkspaces = true, IncludeAddress = true });
        return ccs.First();
    }

    public async Task<CoworkingCenter> AddCenter(CoworkingCenter center)
    {
        var cc = await context.CoworkingCenters.AddAsync(center);
        await context.SaveChangesAsync();
        return cc.Entity;
    }

    public async Task<CoworkingCenter> UpdateCenter(CoworkingCenter center)
    {
        var c = context.CoworkingCenters.Update(center);
        await context.SaveChangesAsync();
        return c.Entity;
    }
}

public class CoworkingCenterFilter : FilterBase
{
     [CompareTo(nameof(CoworkingCenter.CoworkingCenterId))]
     public int? Id { get; set; }

     [StringFilterOptions(StringFilterOption.Contains)]
     [CompareTo(nameof(CoworkingCenter.Name))]
     public string? LikeName { get; set; }

     [StringFilterOptions(StringFilterOption.Contains)]
     [CompareTo(nameof(CoworkingCenter.Description))]
     public string? LikeDescription { get; set; }
     public RangeFilter<decimal> Latitude { get; set; } = new();
     public RangeFilter<decimal> Longitude { get; set; } = new();
     public bool IncludeWorkspaces { get; set; } = false;
     public bool IncludeAddress { get; set; } = false;
}

