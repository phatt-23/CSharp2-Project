using CoworkingApp.Data;
using CoworkingApp.Models.DataModels;
using CoworkingApp.Models.DTOModels.Workspace;
using Microsoft.EntityFrameworkCore;

namespace CoworkingApp.Services;

public class WorkspacesService(CoworkingDbContext context)
{
    /// Returns the filtered and paginated workspaces
    /// and total count mathing the filtering (before pagination).
    public async Task<(IEnumerable<Workspace>, int)> GetAsync(WorkspacesQueryDto query)
    {
        var workspaces = context.Workspaces
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Name))
            workspaces = workspaces.Where(w => w.Name == query.Name);
        if (query.StatusId.HasValue)
            workspaces = workspaces.Where(w => w.StatusId == query.StatusId);

        var totalCount = await workspaces.CountAsync();

        workspaces = workspaces
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .Include(w => w.CoworkingCenter)
            .Include(w => w.Status);

        return (workspaces, totalCount);
    }

    public async Task<Workspace?> GetByIdAsync(int id)
    {
        return await context.Workspaces
            .Where(w => w.Id == id)
            .Include(w => w.CoworkingCenter)
            .Include(w => w.Status)
            .SingleOrDefaultAsync();
    }

    public IEnumerable<WorkspaceHistory> GetHistoriesOfWorkspace(int id)
        => context.WorkspaceHistories
            .Where(w => w.Id == id);

    public async Task<Workspace> CreateAsync(WorkspaceCreateRequestDto request)
    {
        var workspace = new Workspace
        {
            Name = request.Name,
            Description = request.Description,
            StatusId = request.StatusId,
            CoworkingCenterId = request.CoworkingCenterId,
        };
        
        var addedWorkspace = await context.Workspaces.AddAsync(workspace);
        await context.SaveChangesAsync();
        
        return addedWorkspace.Entity;
    }

    public async Task<bool> UpdateStatusAsync(int workspaceId, int statusId)
    {
        var workspace = await context.Workspaces.FindAsync(workspaceId);
        if (workspace == null)
            return false;

        if (!context.WorkspaceStatuses.Any(ws => ws.Id == statusId))
            return false;
        
        workspace.StatusId = statusId;
        context.Update(workspace);

        var history = new WorkspaceHistory
        {
            WorkspaceId = workspaceId,
            StatusId = statusId,
        };

        context.WorkspaceHistories.Add(history);
        
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<Workspace?> RemoveByIdAsync(int id)
    {
        var workspace = await context.Workspaces.FindAsync(id);
        if (workspace is null)
            return null;

        return context.Workspaces.Remove(workspace).Entity;
    }
}
