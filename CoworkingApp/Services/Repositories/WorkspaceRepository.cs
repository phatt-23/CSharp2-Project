using AutoFilterer.Attributes;
using AutoFilterer.Enums;
using AutoFilterer.Extensions;
using AutoFilterer.Types;
using CoworkingApp.Data;
using CoworkingApp.Models.DataModels;
using Microsoft.EntityFrameworkCore;

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
            query = query.Include(w => w.WorkspaceHistories);
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

        if (filter.IncludeLatestPricing)
        {
            var result = query.Include(x => x.WorkspacePricings).ToList();
            result.ForEach(w => w.WorkspacePricings = [.. w.WorkspacePricings.OrderByDescending(p => p.ValidFrom).Take(1)]);

            return result;
        }

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

    public bool IncludeReservations { get; set; } = false;
    public bool IncludeHistories { get; set; } = false;
    public bool IncludeLatestPricing { get; set; } = true;
    public bool IncludePricings { get; set; } = false;
    public bool IncludeStatus { get; set; } = false;
    public bool IncludeCoworkingCenter { get; set; } = false;
    public bool HasPricing { get; set; } = true;
}
