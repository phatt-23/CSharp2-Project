using CoworkingApp.Data;
using CoworkingApp.Models.DataModels;
using CoworkingApp.Models.DTOModels.Workspace;
using Microsoft.EntityFrameworkCore;

namespace CoworkingApp.Services;

public class WorkspacesService(CoworkingDbContext context)
{
    private readonly CoworkingDbContext _context = context;

    /// Returns the filtered and paginated workspaces
    /// and total count mathing the filtering (before pagination).
    public async Task<(ICollection<Workspace>, int)> GetAsync(WorkspacesQueryDto query)
    {
        var workspaces = _context.Workspaces
            .AsQueryable();

        if (query.Id.HasValue)
            workspaces = workspaces.Where(w => w.Id == query.Id);
        
        if (!string.IsNullOrWhiteSpace(query.Name))
            workspaces = workspaces.Where(w => w.Name == query.Name);
        
        if (query.StatusId.HasValue)
            workspaces = workspaces.Where(w => w.StatusId == query.StatusId);

        // count the total mathing the query 
        var totalCount = await workspaces.CountAsync();

        workspaces = workspaces
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Include(w => w.CoworkingCenter)
            .Include(w => w.Status);

        return (workspaces.ToList(), totalCount);
    }

    public async Task<Workspace> CreateAsync(WorkspaceCreateRequestDto workspaceCreateRequestReqeust)
    {
        var workspace = new Workspace
        {
            Name = workspaceCreateRequestReqeust.Name,
            Description = workspaceCreateRequestReqeust.Description,
            StatusId = workspaceCreateRequestReqeust.StatusId,
            CoworkingCenterId = workspaceCreateRequestReqeust.CoworkingCenterId,
        };
        
        var addedWorkspace = await _context.Workspaces.AddAsync(workspace);
        await _context.SaveChangesAsync();
        
        return addedWorkspace.Entity;
    }

    public async Task<bool> UpdateStatusAsync(int workspaceId, int statusId)
    {
        var workspace = await _context.Workspaces.FindAsync(workspaceId);
        if (workspace == null)
            return false;

        if (!_context.WorkspaceStatuses.Any(ws => ws.Id == statusId))
            return false;
        
        workspace.StatusId = statusId;
        _context.Update(workspace);

        var history = new WorkspaceHistory
        {
            WorkspaceId = workspaceId,
            StatusId = statusId,
        };

        _context.WorkspaceHistories.Add(history);
        
        await _context.SaveChangesAsync();
        return true;
    }
}
