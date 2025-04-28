using CoworkingApp.Models.DataModels;
using CoworkingApp.Models.DtoModels;
using CoworkingApp.Models.Exceptions;
using CoworkingApp.Services.Repositories;

namespace CoworkingApp.Services;

public interface IWorkspaceService
{
    Task<IEnumerable<Workspace>> GetWorkspaces(WorkspaceQueryRequestDto request);
    Task<IEnumerable<Workspace>> GetWorkspacesForAdmin(AdminWorkspaceQueryRequestDto request);
    Task<Workspace> GetWorkspaceById(int id);
    Task<IEnumerable<WorkspaceHistory>> GetWorkspaceHistory(int id);
    Task<Workspace> CreateWorkspace(WorkspaceCreateRequestDto request);
    Task<Workspace> UpdateWorkspace(int workspaceId, WorkspaceUpdateRequestDto request);
    Task<bool> UpdateWorkspaceStatus(int workspaceId, WorkspaceStatusType statusType);
    Task<Workspace> RemoveWorkspaceById(int workspaceId);
}

public class WorkspaceService
    (
        IWorkspaceRepository workspaceRepository,
        IWorkspaceHistoryRepository historyRepository,
        IWorkspaceStatusRepository statusRepository
    )
    : IWorkspaceService
{
    public async Task<IEnumerable<Workspace>> GetWorkspacesForAdmin(AdminWorkspaceQueryRequestDto request)
    {
        var workspaces = await workspaceRepository.GetWorkspaces(new WorkspaceFilter
        {
            LikeName = request.LikeName,
            IncludeLatestPricing = true,
            IncludeCoworkingCenter = true,
            IncludeStatus = true,
        });

        return workspaces;
    }

    public async Task<IEnumerable<Workspace>> GetWorkspaces(WorkspaceQueryRequestDto request)
    {
        var workspaces = await workspaceRepository.GetWorkspaces(new WorkspaceFilter
        {
            LikeName = request.LikeName,
            IsRemoved = false,
            IncludeLatestPricing = true,
            IncludeCoworkingCenter = true,
            IncludeStatus = true,
        });

        return workspaces;
    }

    public async Task<Workspace> GetWorkspaceById(int id)
    {
        var workspaces = await workspaceRepository.GetWorkspaces(new WorkspaceFilter
        {
            Id = id,
            IncludeStatus = true,
            IncludeCoworkingCenter = true,
            IncludePricings = true,
            IncludeHistories = true,
            IncludeReservations = true,
        });

        var workspace = workspaces.SingleOrDefault() 
                ?? 
                throw new NotFoundException("Workspace with id '" + id + "' not found or multiple exists.");

        return workspace;
    }

    public async Task<IEnumerable<WorkspaceHistory>> GetWorkspaceHistory(int workspaceId)
    {
        if (!(await workspaceRepository.GetWorkspaces(new WorkspaceFilter() { Id = workspaceId })).Any())
        { 
            throw new InvalidOperationException($"Workspace with id '{workspaceId}' doesn't exist");
        }

        var histories = await historyRepository.GetHistories(new WorkspaceHistoryFilter
        {
            WorkspaceId = workspaceId,
        });

        return histories;
    }

    public async Task<Workspace> CreateWorkspace(WorkspaceCreateRequestDto request)
    {
        var workspace = await workspaceRepository.AddWorkspace(new Workspace
        {
            Name = request.Name,
            Description = request.Description,
            CoworkingCenterId = request.CoworkingCenterId,
        });

        return workspace;
    }

    public async Task<Workspace> UpdateWorkspace(int workspaceId, WorkspaceUpdateRequestDto request)
    {
        var workspace = await GetWorkspaceById(workspaceId);

        if (request.Name != null) workspace.Name = request.Name;
        if (request.Description != null) workspace.Description = request.Description;
        if (request.CoworkingCenterId.HasValue) workspace.CoworkingCenterId = request.CoworkingCenterId.Value;
        
        return await workspaceRepository.UpdateWorkspace(workspace);
    }

    public async Task<bool> UpdateWorkspaceStatus(int workspaceId, WorkspaceStatusType statusType)
    {
        var workspaces = await workspaceRepository.GetWorkspaces(new WorkspaceFilter { Id = workspaceId });
        
        var workspace = workspaces.Single();

        var statuses = await statusRepository.GetStatuses(new WorkspaceStatusFilter { LikeName = statusType.ToString() });
        var status = statuses.Single();
       
        await workspaceRepository.UpdateWorkspace(workspace);
        await historyRepository.AddHistory(new WorkspaceHistory
        {
            WorkspaceId = workspaceId,
            StatusId = status.WorkspaceStatusId,
        });
        
        return true;
    }

    public async Task<Workspace> RemoveWorkspaceById(int workspaceId)
    {
        var workspaces = await workspaceRepository.GetWorkspaces(new WorkspaceFilter { Id = workspaceId });

        var workspace = workspaces.FirstOrDefault()
            ??
            throw new NotFoundException("The workspace with id '" + workspaceId + "' was not found.");
        
        return await workspaceRepository.RemoveWorkspace(workspace);
    }
}

