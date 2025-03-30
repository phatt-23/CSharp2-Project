using CoworkingApp.Data;
using CoworkingApp.Models.DataModels;
using CoworkingApp.Models.DTOModels.CoworkingCenters;
using CoworkingApp.Models.DTOModels.WorkspaceStatus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoworkingApp.Services;

public interface IWorkspaceStatusService
{
    Task<IEnumerable<WorkspaceStatus>> GetWorkspaceStatusesAsync(WorkspaceStatusQueryRequestDto request);
    Task<WorkspaceStatus?> GetWorkspaceStatusByIdAsync(int workspaceStatusId);
}


public class WorkspaceStatusService(IWorkspaceStatusRepository statusRepository) : IWorkspaceStatusService
{
    public async Task<IEnumerable<WorkspaceStatus>> GetWorkspaceStatusesAsync(WorkspaceStatusQueryRequestDto request)
    { 
        var ss = await statusRepository.GetWorkspaceStatusAsync(new WorkspaceStatusFilterOptions
        { 
            Id = request.Id,
            LikeName = request.Name
        });

       return ss;
    }

    public async Task<WorkspaceStatus> GetWorkspaceStatusByIdAsync(int workspaceStatusId)
    {
        var ss = await statusRepository.GetWorkspaceStatusAsync(new WorkspaceStatusFilterOptions
        { 
            Id = workspaceStatusId,
            IncludeWorkspaces = true
        });

        return ss.Single();
    }

}