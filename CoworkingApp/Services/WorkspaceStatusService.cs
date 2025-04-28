using CoworkingApp.Models.DataModels;
using CoworkingApp.Models.DtoModels;
using CoworkingApp.Services.Repositories;

namespace CoworkingApp.Services;

public interface IWorkspaceStatusService
{
    Task<IEnumerable<WorkspaceStatus>> GetStatuses(WorkspaceStatusQueryRequestDto request);
    Task<WorkspaceStatus> GetStatusById(int workspaceStatusId);
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
            LikeName = request.Name
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

}