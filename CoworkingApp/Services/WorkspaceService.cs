using CoworkingApp.Models.DataModels;
using CoworkingApp.Models.DTOModels.Workspace;
using CoworkingApp.Models.Exceptions;

namespace CoworkingApp.Services;

public interface IWorkspaceService
{
    Task<IEnumerable<Workspace>> GetWorkspacesAsync(WorkspaceQueryRequestDto request);
    Task<IEnumerable<Workspace>> GetWorkspacesForAdminAsync(AdminWorkspaceQueryRequestDto request);
    Task<Workspace> GetWorkspaceByIdAsync(int id);
    Task<IEnumerable<WorkspaceHistory>> GetWorkspaceHistoryAsync(int id);
    Task<Workspace> CreateWorkspaceAsync(WorkspaceCreateRequestDto request);
    Task<Workspace> UpdateWorkspaceAsync(int workspaceId, WorkspaceUpdateRequestDto request);
    Task<bool> UpdateWorkspaceStatusAsync(int workspaceId, WorkspaceStatusType statusType);
    Task<Workspace> RemoveWorkspaceByIdAsync(int workspaceId);
}


public class WorkspaceService(
    IWorkspaceRepository workspaceRepository,
    IWorkspaceHistoryRepository workspaceHistoryRepository,
    IWorkspaceStatusRepository statusRepository
) : IWorkspaceService
{
    /// Gets all workspaces
    public async Task<IEnumerable<Workspace>> GetWorkspacesForAdminAsync(AdminWorkspaceQueryRequestDto request)
    {
        var workspaces = await workspaceRepository.GetWorkspacesAsync(new WorkspaceFilter
        {
            LikeName = request.LikeName,
            StatusId = request.StatusId,
            StatusType = request.StatusType,
            IncludeLatestPricing = true,
            IncludeCoworkingCenter = true,
            IncludeStatus = true,
        });

        return workspaces;
    }

    /// Excludes removed workspaces
    public async Task<IEnumerable<Workspace>> GetWorkspacesAsync(WorkspaceQueryRequestDto request)
    {
        var workspaces = await workspaceRepository.GetWorkspacesAsync(new WorkspaceFilter
        {
            LikeName = request.LikeName,
            IsRemoved = false,
            IncludeLatestPricing = true,
            IncludeCoworkingCenter = true,
            IncludeStatus = true,
        });

        return workspaces;
    }

    /// 
    public async Task<Workspace> GetWorkspaceByIdAsync(int id)
    {
        var workspaces = await workspaceRepository.GetWorkspacesAsync(new WorkspaceFilter
        {
            Id = id,
            IncludeStatus = true,
            IncludeCoworkingCenter = true,
        });

        var w = workspaces.SingleOrDefault()
                ?? throw new NotFoundException("Workspace with id '" + id + "' not found or multiple exists.");

        return w;
    }

    public async Task<IEnumerable<WorkspaceHistory>> GetWorkspaceHistoryAsync(int workspaceId)
    {
        if (!await workspaceRepository.WorkspacesExistAsync(new WorkspaceFilter { Id = workspaceId }))
            throw new InvalidOperationException($"Workspace with id '{workspaceId}' doesn't exist");

        var histories = await workspaceHistoryRepository.GetHistoriesAsync(new WorkspaceHistoryFilter
        {
            WorkspaceId = workspaceId,
        });

        return histories;
    }

    public async Task<Workspace> CreateWorkspaceAsync(WorkspaceCreateRequestDto request)
    {
        var workspace = await workspaceRepository.AddWorkspaceAsync(new Workspace
        {
            Name = request.Name,
            Description = request.Description,
            StatusId = request.StatusId,
            CoworkingCenterId = request.CoworkingCenterId,
        });

        return workspace;
    }

    public async Task<Workspace> UpdateWorkspaceAsync(int workspaceId, WorkspaceUpdateRequestDto request)
    {
        var workspace = await GetWorkspaceByIdAsync(workspaceId);

        if (request.Name != null) 
            workspace.Name = request.Name;
        
        if (request.Description != null) 
            workspace.Description = request.Description;
        
        if (request.CoworkingCenterId.HasValue) 
            workspace.CoworkingCenterId = request.CoworkingCenterId.Value;
        
        if (request.StatusId.HasValue) 
            workspace.StatusId = request.StatusId.Value;

        return await workspaceRepository.UpdateWorkspaceAsync(workspace);
    }


    public async Task<bool> UpdateWorkspaceStatusAsync(int workspaceId, WorkspaceStatusType statusType)
    {
        var workspaces = await workspaceRepository.GetWorkspacesAsync(
            new WorkspaceFilter { Id = workspaceId });
        
        var workspace = workspaces.Single();

        var statuses = await statusRepository.GetWorkspaceStatusAsync(
            new WorkspaceStatusFilter { LikeName = statusType.ToString() });
        var status = statuses.Single();
       
        workspace.StatusId = status.Id;

        await workspaceRepository.UpdateWorkspaceAsync(workspace);
        await workspaceHistoryRepository.AddWorkspaceHistoryAsync(new WorkspaceHistory
        {
            WorkspaceId = workspaceId,
            StatusId = status.Id,
        });
        
        return true;
    }

    public async Task<Workspace> RemoveWorkspaceByIdAsync(int workspaceId)
    {
        var ws = await workspaceRepository.GetWorkspacesAsync(new WorkspaceFilter { Id = workspaceId });
        var w = ws.FirstOrDefault()
            ?? throw new NotFoundException("The workspace with id '" + workspaceId + "' was not found.");
        
        return await workspaceRepository.RemoveWorkspaceAsync(w);
    }
}

