using CoworkingApp.Data;
using CoworkingApp.Models.DataModels;
using CoworkingApp.Models.DTOModels.CoworkingCenters;
using CoworkingApp.Models.DTOModels.WorkspaceStatus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoworkingApp.Services;

public class WorkspaceStatusesService(CoworkingDbContext context)
{

    public async Task<(ICollection<WorkspaceStatus>, int)> GetAsync(WorkspaceStatusQueryDto query)
    {
        var workspaceStatuses = context.WorkspaceStatuses.AsQueryable();

        if (query.Id.HasValue)
        {
            workspaceStatuses = workspaceStatuses.Where(ws => ws.Id == query.Id);
        }

        if (query.Name != null)
        {
            workspaceStatuses = workspaceStatuses.Where(ws => ws.Name == query.Name);
        }
      
        var totalCount = workspaceStatuses.Count();
        
        return (await workspaceStatuses.ToListAsync(), totalCount);
    }

    public async Task<WorkspaceStatus?> GetByIdAsync(int id)
    {
        var workspaceStatus = await context.WorkspaceStatuses.FindAsync(id);
        return workspaceStatus;
    }

}