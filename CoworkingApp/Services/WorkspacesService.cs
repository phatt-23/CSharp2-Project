using AutoMapper;
using CoworkingApp.Data;
using CoworkingApp.Models.DataModels;
using CoworkingApp.Models.DTOModels.Workspace;
using CoworkingApp.Models.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace CoworkingApp.Services;

public interface IWorkspaceService
{
    Task<IEnumerable<Workspace>> GetWorkspacesForAdminAsync(AdminWorkspaceQueryRequestDto request);
    Task<IEnumerable<Workspace>> GetWorkspacesAsync(WorkspaceQueryRequestDto request);

    Task<Workspace?> GetWorkspaceByIdAsync(int id);
    Task<IEnumerable<WorkspaceHistory>> GetWorkspaceHistoryAsync(int id);
    Task<Workspace> CreateWorkspaceAsync(WorkspaceCreateRequestDto request);
    Task<bool> UpdateWorkspaceStatusAsync(int workspaceId, WorkspaceStatusType statusType);
    Task<Workspace?> RemoveWorkspaceByIdAsync(int workspaceId);
}


public class WorkspaceService(
    IWorkspaceRepository workspaceRepository,
    IWorkspaceHistoryRepository historyRepository,
    IWorkspaceStatusRepository statusRepository,
    CoworkingDbContext context,
    IMapper mapper
    ) : IWorkspaceService
{
    public Task<IEnumerable<Workspace>> GetWorkspacesForAdminAsync(AdminWorkspaceQueryRequestDto request)
    {
        var workspaces = workspaceRepository.GetWorkspacesAsync(new WorkspaceFilterOptions()
        {
            LikeName = request.Name,
            StatusId = request.StatusId,
            StatusType = request.StatusType,
            IncludeStatus = true,
            IncludeCoworkingCenter = true,
        });
        
        return workspaces;
    }
    
    public Task<IEnumerable<Workspace>> GetWorkspacesAsync(WorkspaceQueryRequestDto request)
    {
        var workspaces = workspaceRepository.GetWorkspacesAsync(new WorkspaceFilterOptions()
        {
            LikeName = request.Name,
            IncludeStatus = true,
            IncludeCoworkingCenter = true,
        });

        return workspaces;
    }

    public async Task<Workspace> GetWorkspaceByIdAsync(int id)
    {
        var workspaces = await workspaceRepository.GetWorkspacesAsync(new WorkspaceFilterOptions
        {
            Id = id,
            IncludeStatus = true,
            IncludeCoworkingCenter = true,
        });

        var w = workspaces.SingleOrDefault();
        if (w == null) 
            throw new NotFoundException("Workspace with id '" + id + "' not found or multiple exists.");
        
        return w;
    }

    public async Task<IEnumerable<WorkspaceHistory>> GetWorkspaceHistoryAsync(int workspaceId)
    {
        if (!await workspaceRepository.WorkspacesExistAsync(new WorkspaceFilterOptions { Id = workspaceId }))
            throw new InvalidOperationException($"Workspace with id '{workspaceId}' doesn't exist");

        var histories = await historyRepository.GetHistoriesAsync(new WorkspaceHistoryFilterOptions
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

    public async Task<bool> UpdateWorkspaceStatusAsync(int workspaceId, WorkspaceStatusType statusType)
    {
        var workspaces = await workspaceRepository.GetWorkspacesAsync(
            new WorkspaceFilterOptions { Id = workspaceId });
        
        var workspace = workspaces.Single();

        var statuses = await statusRepository.GetWorkspaceStatusAsync(
            new WorkspaceStatusFilterOptions { LikeName = statusType.ToString() });
        var status = statuses.Single();
       
        workspace.StatusId = status.Id;

        await workspaceRepository.UpdateWorkspaceAsync(workspace);
        await historyRepository.AddWorkspaceHistoryAsync(new WorkspaceHistory
        {
            WorkspaceId = workspaceId,
            StatusId = status.Id,
        });
        
        return true;
    }

    public async Task<Workspace> RemoveWorkspaceByIdAsync(int workspaceId)
    {
        var w = (await workspaceRepository.GetWorkspacesAsync(new WorkspaceFilterOptions { Id = workspaceId })).FirstOrDefault();
        if (w == null)
            throw new NotFoundException("The workspace with id '" + workspaceId + "' was not found.");
        
        return await workspaceRepository.RemoveWorkspaceAsync(w);
    }
}
