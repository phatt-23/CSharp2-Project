using AutoFilterer.Attributes;
using AutoFilterer.Enums;
using AutoFilterer.Extensions;
using AutoFilterer.Types;
using CoworkingApp.Data;
using CoworkingApp.Models.DataModels;
using Microsoft.EntityFrameworkCore;

namespace CoworkingApp.Services;

public interface IWorkspaceStatusRepository
{
    Task<IEnumerable<WorkspaceStatus>> GetWorkspaceStatusAsync(WorkspaceStatusFilter filter);
}

public class WorkspaceStatusRepository(CoworkingDbContext context) : IWorkspaceStatusRepository
{
    public Task<IEnumerable<WorkspaceStatus>> GetWorkspaceStatusAsync(WorkspaceStatusFilter filter)
    {
        var query = context.WorkspaceStatuses.ApplyFilter(filter);

        if (filter.IncludeWorkspaceHistories)
            query = query.Include(s => s.WorkspaceHistories);
        
        if (filter.IncludeWorkspaces)
            query = query.Include(s => s.Workspaces);
        
        return Task.FromResult<IEnumerable<WorkspaceStatus>>(query);
    }
}

public class WorkspaceStatusFilter : FilterBase
{
    [CompareTo(nameof(WorkspaceStatus.Id))]
    public int? Id { get; set; }

    [CompareTo(nameof(WorkspaceStatus.Name))]
    [StringFilterOptions(StringFilterOption.Contains)]
    public string? LikeName { get; set; }

    public bool IncludeWorkspaceHistories { get; set; } = false;
    public bool IncludeWorkspaces { get; set; } = false;
}
