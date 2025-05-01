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
    Task<Workspace> UpdateWorkspace(WorkspaceUpdateRequestDto request);
    Task<bool> UpdateWorkspaceStatus(int workspaceId, WorkspaceStatusType statusType);
    Task<Workspace> RemoveWorkspaceById(int workspaceId);
}

public class WorkspaceService
    (
        IWorkspaceRepository workspaceRepository,
        IWorkspaceHistoryRepository historyRepository,
        IWorkspaceStatusRepository statusRepository,
        IWorkspaceStatusService statusService,
        IWorkspacePricingRepository pricingRepository,
        ICoworkingCenterRepository coworkingCenterRepository
    )
    : IWorkspaceService
{
    public async Task<IEnumerable<Workspace>> GetWorkspacesForAdmin(AdminWorkspaceQueryRequestDto request)
    {
        return await GetWorkspaces(request);
    }

    public async Task<IEnumerable<Workspace>> GetWorkspaces(WorkspaceQueryRequestDto request)
    {
        var workspaces = await workspaceRepository.GetWorkspaces(new WorkspaceFilter
        {
            LikeName = request.NameContains,
            IsRemoved = false,
            IncludePricings = true,
            IncludeCoworkingCenter = true,
            IncludeHistories = true,
            IncludeStatus = true,
            PricePerHour = request.PricePerHour,
            CoworkingCenterId = request.CoworkingCenterId
        });
        
        if (request.Status != null)
        {
            workspaces = workspaces.Where(w =>
            {
                var currentHistory = w.GetCurrentHistory();
                return currentHistory == null
                    ? throw new Exception("Workspace doesn't have status history")
                    : currentHistory.Status.Type == request.Status.Value;
            });
        }

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

        if (!workspaces.Any())
        {
            throw new NotFoundException($"Workspace with id '{id}' not found or multiple exists.");
        }

        return workspaces.Single();
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
        // check for existence will throw if not
        await coworkingCenterRepository.GetCenterById(request.CoworkingCenterId);

        var workspace = await workspaceRepository.AddWorkspace(new Workspace
        {
            Name = request.Name,
            Description = request.Description,
            CoworkingCenterId = request.CoworkingCenterId,
        });

        var status = await statusService.GetStatusByType(request.Status);

        await historyRepository.AddHistory(new WorkspaceHistory
        {
            WorkspaceId = workspace.WorkspaceId,
            StatusId = status.WorkspaceStatusId,
        });

        await pricingRepository.AddPricing(new WorkspacePricing
        {
            WorkspaceId = workspace.WorkspaceId,
            PricePerHour = request.PricePerHour,
            ValidFrom = DateTime.UtcNow,
            ValidUntil = null,
        });

        return workspace;
    }

    public async Task<Workspace> UpdateWorkspace(WorkspaceUpdateRequestDto request)
    {
        var workspace = await GetWorkspaceById(request.WorkspaceId);

        workspace.CoworkingCenterId = request.CoworkingCenterId;

        if (!string.IsNullOrEmpty(request.Name)) 
            workspace.Name = request.Name;

        if (!string.IsNullOrEmpty(request.Description)) 
            workspace.Description = request.Description;
        
        return await workspaceRepository.UpdateWorkspace(workspace);
    }

    public async Task<bool> UpdateWorkspaceStatus(int workspaceId, WorkspaceStatusType statusType)
    {
        var workspaces = await workspaceRepository.GetWorkspaces(new WorkspaceFilter { Id = workspaceId });
        
        var workspace = workspaces.Single();

        var statuses = await statusRepository.GetStatuses(new WorkspaceStatusFilter { NameContains = statusType.ToString() });
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

