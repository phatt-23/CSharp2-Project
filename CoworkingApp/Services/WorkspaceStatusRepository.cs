using CoworkingApp.Data;
using CoworkingApp.Models.DataModels;
using Microsoft.EntityFrameworkCore;

namespace CoworkingApp.Services;

public interface IWorkspaceStatusRepository
{
    Task<IEnumerable<WorkspaceStatus>> GetWorkspaceStatusAsync(WorkspaceStatusFilterOptions options);
}

public class WorkspaceStatusRepository(CoworkingDbContext context) : IWorkspaceStatusRepository
{
    public Task<IEnumerable<WorkspaceStatus>> GetWorkspaceStatusAsync(WorkspaceStatusFilterOptions options)
    {
        var ss = context.WorkspaceStatuses
            .Where(w => options.Id == null || options.Id == w.Id)
            .Where(w => options.LikeName == null || w.Name.Contains(options.LikeName));

        if (options.IncludeWorkspaceHistories)
            ss = ss.Include(s => s.WorkspaceHistories);
        
        if (options.IncludeWorkspaces)
            ss = ss.Include(s => s.Workspaces);
        
        return Task.FromResult<IEnumerable<WorkspaceStatus>>(ss);
    }
}

public class WorkspaceStatusFilterOptions
{
    public int? Id { get; set; }
    public string? LikeName { get; set; }
    public bool IncludeWorkspaceHistories { get; set; } = false;
    public bool IncludeWorkspaces { get; set; } = false;
}
