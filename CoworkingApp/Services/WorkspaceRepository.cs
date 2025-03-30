using CoworkingApp.Data;
using CoworkingApp.Models.DataModels;
using Microsoft.EntityFrameworkCore;

namespace CoworkingApp.Services;

public interface IWorkspaceRepository
{
    Task<IEnumerable<Workspace>> GetWorkspacesAsync(WorkspaceFilterOptions options);
    Task<Workspace> AddWorkspaceAsync(Workspace workspace);
    Task<Workspace> RemoveWorkspaceAsync(Workspace workspace);

    Task<Workspace> UpdateWorkspaceAsync(Workspace workspace);
    
    Task<bool> WorkspacesExistAsync(WorkspaceFilterOptions options);
}

public class WorkspaceRepository(CoworkingDbContext context) : IWorkspaceRepository
{
    public Task<IEnumerable<Workspace>> GetWorkspacesAsync(WorkspaceFilterOptions options)
    {
        var ws = context.Workspaces
            .Where(w => options.Id == null || w.Id == options.Id)
            .Where(w => options.LikeName == null || w.Name.Contains(options.LikeName))
            .Where(w => options.LikeDescription == null || w.Description.Contains(options.LikeDescription))
            .Where(w => options.CoworkingCenterId == null || w.CoworkingCenterId == options.CoworkingCenterId)
            .Where(w => options.StatusId == null || w.StatusId == options.StatusId)
            .Where(w => options.StatusType == null || w.Status.Name == options.StatusType.ToString())
            .Where(w => options.IsRemoved == null || w.IsRemoved == options.IsRemoved)
            .Where(w => options.CreatedAtLow == null || w.CreatedAt >= options.CreatedAtLow)
            .Where(w => options.CreatedAtHigh == null || w.CreatedAt <= options.CreatedAtHigh);

        if (options.IncludeReservations)
            ws = ws.Include(w => w.Reservations);
        
        if (options.IncludeHistories)
            ws = ws.Include(w => w.WorkspaceHistories);
        
        if (options.IncludePricings)
            ws = ws.Include(w => w.WorkspacePricings);

        if (options.IncludeCoworkingCenter)
            ws = ws.Include(w => w.CoworkingCenter);
            
        if (options.IncludeStatus)
            ws = ws.Include(w => w.Status);
        
        return Task.FromResult<IEnumerable<Workspace>>(ws); 
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

    public async Task<bool> WorkspacesExistAsync(WorkspaceFilterOptions options)
    {
        return (await GetWorkspacesAsync(options)).Any();
    }
}

public class WorkspaceFilterOptions
{
    public int? Id { get; set; }
    public string? LikeName { get; set; }
    public string? LikeDescription { get; set; }
    public int? CoworkingCenterId { get; set; }
    public int? StatusId { get; set; }
    public WorkspaceStatusType? StatusType { get; set; }
    public bool? IsRemoved { get; set; }
    public DateTime? CreatedAtLow { get; set; }
    public DateTime? CreatedAtHigh { get; set; }
    public bool IncludeReservations { get; set; } = false;
    public bool IncludeHistories { get; set; } = false;
    public bool IncludePricings { get; set; } = false;
    public bool IncludeStatus { get; set; } = false;
    public bool IncludeCoworkingCenter { get; set; } = false;
}
