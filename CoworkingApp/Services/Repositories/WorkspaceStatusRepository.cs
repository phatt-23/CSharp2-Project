using AutoFilterer.Attributes;
using AutoFilterer.Enums;
using AutoFilterer.Extensions;
using AutoFilterer.Types;
using CoworkingApp.Data;
using CoworkingApp.Models.DataModels;
using Microsoft.EntityFrameworkCore;

namespace CoworkingApp.Services.Repositories;

public interface IWorkspaceStatusRepository
{
    Task<IEnumerable<WorkspaceStatus>> GetStatuses(WorkspaceStatusFilter filter);
}

public class WorkspaceStatusRepository
    (
        CoworkingDbContext context
    ) 
    : IWorkspaceStatusRepository
{
    public Task<IEnumerable<WorkspaceStatus>> GetStatuses(WorkspaceStatusFilter filter)
    {
        var query = context.WorkspaceStatuses.ApplyFilter(filter);

        if (filter.IncludeWorkspaceHistories)
            query = query.Include(s => s.WorkspaceHistories);
        
        return Task.FromResult<IEnumerable<WorkspaceStatus>>(query);
    }
}

public class WorkspaceStatusFilter : FilterBase
{
    [CompareTo(nameof(WorkspaceStatus.WorkspaceStatusId))]
    public int? Id { get; set; }

    [CompareTo(nameof(WorkspaceStatus.Name))]
    [StringFilterOptions(StringFilterOption.Contains)]
    public string? LikeName { get; set; }

    public bool IncludeWorkspaceHistories { get; set; } = false;
    public bool IncludeWorkspaces { get; set; } = false;
}
