using System.ComponentModel.DataAnnotations;
using AutoFilterer.Attributes;
using AutoFilterer.Extensions;
using AutoFilterer.Types;
using CoworkingApp.Data;
using CoworkingApp.Models.DataModels;
using Microsoft.EntityFrameworkCore;

namespace CoworkingApp.Services;


public interface IWorkspaceHistoryRepository
{
    Task<IEnumerable<WorkspaceHistory>> GetHistoriesAsync(WorkspaceHistoryFilter filter);
    Task<WorkspaceHistory> AddWorkspaceHistoryAsync(WorkspaceHistory workspaceHistory);
}

public class WorkspaceWorkspaceHistoryRepository(CoworkingDbContext context) : IWorkspaceHistoryRepository
{
    public Task<IEnumerable<WorkspaceHistory>> GetHistoriesAsync(WorkspaceHistoryFilter filter)
    {
        var query = context.WorkspaceHistories.ApplyFilter(filter);

        query = filter.CreatedAt.ApplyTo(query, x => x.CreatedAt);

        if (filter.IncludeStatus)
            query = query.Include(h => h.Status);

        if (filter.IncludeWorkspace)
            query = query.Include(h => h.Workspace);

        return Task.FromResult<IEnumerable<WorkspaceHistory>>(query);
    }

    public async Task<WorkspaceHistory> AddWorkspaceHistoryAsync(WorkspaceHistory workspaceHistory)
    {
        var h = await context.WorkspaceHistories.AddAsync(workspaceHistory);
        await context.SaveChangesAsync();
        return h.Entity;
    }
}


public class WorkspaceHistoryFilter : FilterBase
{
    [CompareTo(nameof(WorkspaceHistory.Id))]
    public int? Id { get; set; }
    [CompareTo(nameof(WorkspaceHistory.WorkspaceId))]
    public int? WorkspaceId { get; set; }
    [CompareTo(nameof(WorkspaceHistory.StatusId))]
    public int? StatusId { get; set; }
    [CompareTo(nameof(WorkspaceHistory.Status.Type))]
    public WorkspaceStatusType? StatusType { get; set; }

    public RangeFilter<DateTime> CreatedAt { get; set; } = new();

    public bool IncludeStatus { get; set; } = false;
    public bool IncludeWorkspace { get; set; } = false;
}
