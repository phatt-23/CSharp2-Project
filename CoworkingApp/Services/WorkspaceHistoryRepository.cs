using CoworkingApp.Data;
using CoworkingApp.Models.DataModels;
using Microsoft.EntityFrameworkCore;

namespace CoworkingApp.Services;


public interface IWorkspaceHistoryRepository
{
    Task<IEnumerable<WorkspaceHistory>> GetHistoriesAsync(WorkspaceHistoryFilterOptions options);
    Task<WorkspaceHistory> AddWorkspaceHistoryAsync(WorkspaceHistory workspaceHistory);
    
}

public class WorkspaceHistoryRepository(CoworkingDbContext context) : IWorkspaceHistoryRepository
{
    public Task<IEnumerable<WorkspaceHistory>> GetHistoriesAsync(WorkspaceHistoryFilterOptions options)
    {
        var hs = context.WorkspaceHistories
                .Where(w => options.Id == null || options.Id == w.Id)
                .Where(w => options.WorkspaceId == null || options.WorkspaceId == w.Id)
                .Where(w => options.CreatedAtLow == null || w.CreatedAt >= options.CreatedAtLow)
                .Where(w => options.CreatedAtHigh == null || w.CreatedAt >= options.CreatedAtHigh)
                .Where(w => options.StatusId == null || options.StatusId == w.StatusId)
                .Where(w => options.StatusType == null || options.StatusType.ToString() == w.Status.Name);

        if (options.IncludeStatus)
            hs = hs.Include(h => h.Status);

        if (options.IncludeWorkspace)
            hs = hs.Include(h => h.Workspace);

        return Task.FromResult<IEnumerable<WorkspaceHistory>>(hs);
    }

    public async Task<WorkspaceHistory> AddWorkspaceHistoryAsync(WorkspaceHistory workspaceHistory)
    {
        var h = await context.WorkspaceHistories.AddAsync(workspaceHistory);
        await context.SaveChangesAsync();
        return h.Entity;
    }
}


public class WorkspaceHistoryFilterOptions
{
    public int? Id { get; set; }
    public int? WorkspaceId { get; set; }
    public int? StatusId { get; set; }
    public WorkspaceStatusType? StatusType { get; set; }
    public DateTime? CreatedAtLow { get; set; }
    public DateTime? CreatedAtHigh { get; set; }
    public bool IncludeStatus { get; set; } = false;
    public bool IncludeWorkspace { get; set; } = false;
}
