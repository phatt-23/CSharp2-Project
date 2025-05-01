using CoworkingApp.Models.DataModels;
using CoworkingApp.Models.DtoModels;
using CoworkingApp.Services.Repositories;

namespace CoworkingApp.Services;

public interface IWorkspaceStatusService
{
    Task<IEnumerable<WorkspaceStatus>> GetStatuses(WorkspaceStatusQueryRequestDto request);
    Task<WorkspaceStatus> GetStatusById(int workspaceStatusId);
    Task<WorkspaceStatus> GetStatusByType(WorkspaceStatusType type);
}

public class WorkspaceStatusService
    (
        IWorkspaceStatusRepository statusRepository
    )
    : IWorkspaceStatusService
{
    public async Task<IEnumerable<WorkspaceStatus>> GetStatuses(WorkspaceStatusQueryRequestDto request)
    { 
        var statuses = await statusRepository.GetStatuses(new WorkspaceStatusFilter
        { 
            Id = request.Id,
            NameContains = request.Name
        });

        return statuses;
    }

    public async Task<WorkspaceStatus> GetStatusById(int workspaceStatusId)
    {
        var statuses = await statusRepository.GetStatuses(new WorkspaceStatusFilter
        { 
            Id = workspaceStatusId,
            IncludeWorkspaces = true
        });

        return statuses.Single();
    }

    public async Task<WorkspaceStatus> GetStatusByType(WorkspaceStatusType type)
    {
        var status = (await statusRepository.GetStatuses(new WorkspaceStatusFilter
        {
            NameContains = type.ToString()
        })).SingleOrDefault();

        if (status == null)
        {
            throw new Exception($"Status with type {type} not found");
        }

        return status;
    }
}