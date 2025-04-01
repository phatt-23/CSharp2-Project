using AutoFilterer.Attributes;
using AutoFilterer.Enums;
using AutoFilterer.Extensions;
using AutoFilterer.Types;
using CoworkingApp.Data;
using CoworkingApp.Models.DataModels;
using Microsoft.EntityFrameworkCore;

namespace CoworkingApp.Services;

public interface IWorkspaceRepository
{
    Task<IEnumerable<Workspace>> GetWorkspacesAsync(WorkspaceFilter filter);
    Task<Workspace> AddWorkspaceAsync(Workspace workspace);
    Task<Workspace> RemoveWorkspaceAsync(Workspace workspace);
    Task<Workspace> UpdateWorkspaceAsync(Workspace workspace);
    Task<bool> WorkspacesExistAsync(WorkspaceFilter filter);
}

public class WorkspaceRepository(CoworkingDbContext context) : IWorkspaceRepository
{
    public Task<IEnumerable<Workspace>> GetWorkspacesAsync(WorkspaceFilter filter)
    {
        var query = context.Workspaces.ApplyFilter(filter);

        query = filter.CreatedAt.ApplyTo(query, x => x.CreatedAt);

        if (filter.IncludeReservations)
            query = query.Include(w => w.Reservations);
        
        if (filter.IncludeHistories)
            query = query.Include(w => w.WorkspaceHistories);
        
        if (filter.IncludeCoworkingCenter)
            query = query.Include(w => w.CoworkingCenter);
            
        if (filter.IncludeStatus)
            query = query.Include(w => w.Status);

        if (filter.IncludePricings)
            query = query.Include(w => w.WorkspacePricings);
        
        if (filter.IncludeLatestPricing)
        {
            query = query.Select(w => new Workspace
            {
                Id = w.Id,
                Name = w.Name,
                Description = w.Description,
                CoworkingCenter = w.CoworkingCenter,
                WorkspacePricings = w.WorkspacePricings
                    .OrderByDescending(p => p.ValidFrom)
                    .Take(1) // Only get the latest pricing
                    .ToList()
            });
        }
        
        return Task.FromResult<IEnumerable<Workspace>>(query); 
    }

    public async Task<Workspace> AddWorkspaceAsync(Workspace workspace)
    {
        var w = await context.Workspaces.AddAsync(workspace);
        await context.SaveChangesAsync();
        return w.Entity;
    }
    
    public async Task<Workspace> RemoveWorkspaceAsync(Workspace workspace)
    {
        var w = context.Workspaces.Remove(workspace);
        await context.SaveChangesAsync();
        return w.Entity;
    }

    public async Task<Workspace> UpdateWorkspaceAsync(Workspace workspace)
    {
        var w = context.Workspaces.Update(workspace);
        await context.SaveChangesAsync();
        return w.Entity;
    }

    public async Task<bool> WorkspacesExistAsync(WorkspaceFilter filter)
    {
        return (await GetWorkspacesAsync(filter)).Any();
    }
}

public class WorkspaceFilter : FilterBase
{
    [CompareTo(nameof(Workspace.Id))]
    public int? Id { get; set; }

    [CompareTo(nameof(Workspace.Name))]
    [StringFilterOptions(StringFilterOption.Contains)]
    public string? LikeName { get; set; }

    [CompareTo(nameof(Workspace.Description))]
    [StringFilterOptions(StringFilterOption.Contains)]
    public string? LikeDescription { get; set; }
    
    [CompareTo(nameof(Workspace.CoworkingCenterId))]
    public int? CoworkingCenterId { get; set; }

    [CompareTo(nameof(Workspace.StatusId))]
    public int? StatusId { get; set; }

    [CompareTo(nameof(Workspace.Status.Type))]
    public WorkspaceStatusType? StatusType { get; set; }

    [CompareTo(nameof(Workspace.IsRemoved))]
    public bool? IsRemoved { get; set; }

    // [CompareTo(nameof(Workspace.CreatedAt))]
    public RangeFilter<DateTime> CreatedAt { get; set; } = new();

    public bool IncludeReservations { get; set; } = false;
    public bool IncludeHistories { get; set; } = false;
    public bool IncludeLatestPricing { get; set; } = true;
    public bool IncludePricings { get; set; } = false;

    public bool IncludeStatus { get; set; } = false;
    public bool IncludeCoworkingCenter { get; set; } = false;
}
