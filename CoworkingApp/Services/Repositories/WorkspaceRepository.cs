using AutoFilterer.Attributes;
using AutoFilterer.Enums;
using AutoFilterer.Extensions;
using AutoFilterer.Types;
using CoworkingApp.Data;
using CoworkingApp.Models.DataModels;
using CoworkingApp.Models.DtoModels;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Xml.XPath;

namespace CoworkingApp.Services.Repositories;

public interface IWorkspaceRepository
{
    Task<IEnumerable<Workspace>> GetWorkspaces(WorkspaceFilter filter);
    Task<Workspace> AddWorkspace(Workspace workspace);
    Task<Workspace> RemoveWorkspace(Workspace workspace);
    Task<Workspace> UpdateWorkspace(Workspace workspace);
}

public class WorkspaceRepository
    (
        CoworkingDbContext context
    ) 
    : IWorkspaceRepository
{
    public async Task<IEnumerable<Workspace>> GetWorkspaces(WorkspaceFilter filter)
    {
        var query = context.Workspaces.ApplyFilter(filter);

        if (filter.IncludeReservations)
        {
            query = query.Include(w => w.Reservations);
        }

        if (filter.IncludeHistories)
        {
            query = query
                .Include(w => w.WorkspaceHistories)
                .ThenInclude(wh => wh.Status);
        }

        if (filter.IncludeCoworkingCenter)
        {
            query = query
                .Include(w => w.CoworkingCenter)
                .Include(w => w.CoworkingCenter.Address)
                .Include(w => w.CoworkingCenter.Address.City)
                .Include(w => w.CoworkingCenter.Address.City.Country);
        }

        if (filter.IncludePricings)
        {
            query = query.Include(w => w.WorkspacePricings);
        }

        if (filter.HasPricing)
        {
            query = query.Where(w => w.WorkspacePricings.Any());
        }

        if (filter.PricePerHour.Max != null)
        {
            query = query.Where(w => w.WorkspacePricings
                .OrderByDescending(wp => wp.PricePerHour)
                .First().PricePerHour <= filter.PricePerHour.Max);
        }

        if (filter.PricePerHour.Min != null)
        {

            query = query.Where(w => w.WorkspacePricings
                .OrderByDescending(wp => wp.PricePerHour)
                .First().PricePerHour >= filter.PricePerHour.Min);
        }

        if (filter.Status != null)
        {
            var workspaces = await query.Include(w => w.WorkspaceHistories).ThenInclude(wh => wh.Status).ToListAsync();
            query = workspaces.Where(w => w.GetCurrentStatus().Type == filter.Status).AsQueryable();
        }

        query = filter.Sort switch
        {
            WorkspaceSort.PriceDescending => query.OrderByDescending(w => w.WorkspacePricings.OrderByDescending(p => p.ValidFrom).FirstOrDefault()!.PricePerHour),
            WorkspaceSort.PriceAscending => query.OrderBy(w => w.WorkspacePricings.OrderByDescending(p => p.ValidFrom).FirstOrDefault()!.PricePerHour),
            WorkspaceSort.None => query,
            _ => throw new UnreachableException()
        };

        return query;
    }

    public async Task<Workspace> AddWorkspace(Workspace workspace)
    {
        var addedWorkspace = await context.Workspaces.AddAsync(workspace);
        await context.SaveChangesAsync();
        return addedWorkspace.Entity;
    }
    
    public async Task<Workspace> RemoveWorkspace(Workspace workspace)
    {
        workspace.IsRemoved = true;
        return await UpdateWorkspace(workspace);
    }

    public async Task<Workspace> UpdateWorkspace(Workspace workspace)
    {
        var updatedWorkspace = context.Workspaces.Update(workspace);
        await context.SaveChangesAsync();
        return updatedWorkspace.Entity;
    }
}
public enum WorkspaceSort
{
    None = 0,
    PriceDescending,
    PriceAscending,
}

public class WorkspaceFilter : FilterBase
{
    [CompareTo(nameof(Workspace.WorkspaceId))]
    public int? Id { get; set; }

    [CompareTo(nameof(Workspace.Name))]
    [StringFilterOptions(StringFilterOption.Contains)]
    public string? LikeName { get; set; }

    [CompareTo(nameof(Workspace.Description))]
    [StringFilterOptions(StringFilterOption.Contains)]
    public string? LikeDescription { get; set; }
    
    [CompareTo(nameof(Workspace.CoworkingCenterId))]
    public int? CoworkingCenterId { get; set; }

    [CompareTo(nameof(Workspace.IsRemoved))]
    public bool? IsRemoved { get; set; }

    public NullableRangeFilter<decimal> PricePerHour { get; set; } = new();

    public bool IncludeReservations { get; set; } = false;
    public bool IncludeHistories { get; set; } = false;
    public bool IncludeLatestPricing { get; set; } = true;
    public bool IncludePricings { get; set; } = false;
    public bool IncludeStatus { get; set; } = false;
    public bool IncludeCoworkingCenter { get; set; } = false;
    public bool HasPricing { get; set; } = true;

    public WorkspaceStatusType? Status { get; set; }

    public WorkspaceSort Sort { get; set; } = WorkspaceSort.None;
}
