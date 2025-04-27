using CoworkingApp.Models.DataModels;
using CoworkingApp.Models.DTOModels.WorkspaceStatus;
using CoworkingApp.Services.Repositories;

namespace CoworkingApp.Services;

public interface IWorkspaceStatusService
{
    Task<IEnumerable<WorkspaceStatus>> GetWorkspaceStatusesAsync(WorkspaceStatusQueryRequestDto request);
    Task<WorkspaceStatus> GetWorkspaceStatusByIdAsync(int workspaceStatusId);
}


public class WorkspaceStatusService
    (
    IWorkspaceStatusRepository statusRepository
    ) 
    : IWorkspaceStatusService
{
    public async Task<IEnumerable<WorkspaceStatus>> GetWorkspaceStatusesAsync(WorkspaceStatusQueryRequestDto request)
    { 
        var ss = await statusRepository.GetWorkspaceStatusAsync(new WorkspaceStatusFilter
        { 
            Id = request.Id,
            LikeName = request.Name
        });

       return ss;
    }

    public async Task<WorkspaceStatus> GetWorkspaceStatusByIdAsync(int workspaceStatusId)
    {
        var ss = await statusRepository.GetWorkspaceStatusAsync(new WorkspaceStatusFilter
        { 
            Id = workspaceStatusId,
            IncludeWorkspaces = true
        });

        return ss.Single();
    }

}