using CoworkingApp.Data;
using CoworkingApp.Models.DataModels;
using CoworkingApp.Models.DtoModels;
using CoworkingApp.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoworkingApp.Controllers.ApiEndpointContollers.Admin;

[AdminApiController]
[Authorize(Roles = "Admin")]
[Route("/api/admin/stats")]
public class AdminStatisticsApiController
    (
        CoworkingDbContext db
    )
    : Controller
{

    [HttpGet("workspace/revenue")]
    public async Task<ActionResult<WorkspaceRevenuesResponseDto>> GetWorkspaceRevenues([FromQuery] TimeBack timeBack = TimeBack.LastMonth)
    {
        var timeBackDateTime = timeBack.ToDateTime();
        var workspaces = await db.Workspaces.Include(w => w.Reservations).ToListAsync();
        var reveues = workspaces.Select(w => CreateRevenueReport(w, timeBackDateTime)).ToList();

        return Ok(new WorkspaceRevenuesResponseDto
        {
            TimeBack = timeBack,
            Revenues = reveues,
        });
    }

    [HttpGet("workspace/{id:int}/revenue")]
    public async Task<ActionResult<WorkspaceRevenuesResponseDto>> GetWorkspaceRevenue(int id, [FromQuery] TimeBack timeBack = TimeBack.LastMonth)
    {
        var timeBackDateTime = timeBack.ToDateTime();

        var workspace = await db.Workspaces
            .Include(w => w.Reservations)
            .Where(w => w.WorkspaceId == id)
            .FirstOrDefaultAsync();

        if (workspace == null)
        {
            return NotFound("Workspace not found!");
        }

        var revenue = CreateRevenueReport(workspace, timeBackDateTime);
        return Ok(new WorkspaceRevenueResponseDto { TimeBack = timeBack, Revenue = revenue });
    }

    [HttpGet("coworking-center/revenue")]
    public async Task<ActionResult<WorkspaceRevenuesResponseDto>> GetCoworkingCenterRevenues([FromQuery] TimeBack timeBack = TimeBack.LastMonth)
    {
        var timeBackDateTime = timeBack.ToDateTime();

        var centers = await db.CoworkingCenters.Include(cc => cc.Workspaces).ThenInclude(w => w.Reservations).ToListAsync();
        var reveues = centers.Select(cc => CreateCenterRevenueReport(cc, timeBackDateTime)).ToList();

        return Ok(new CoworkingCenterRevenuesResponseDto
        {
            TimeBack = timeBack,
            Revenues = reveues,
        });
    }


    [HttpGet("coworking-center/{id:int}/revenue")]
    public async Task<ActionResult<WorkspaceRevenuesResponseDto>> GetCoworkingCenterRevenue(int id, [FromQuery] TimeBack timeBack = TimeBack.LastMonth)
    {
        var timeBackDateTime = timeBack.ToDateTime();

        var center = await db.CoworkingCenters.Include(c => c.Workspaces).ThenInclude(w => w.Reservations).FirstOrDefaultAsync();

        if (center == null)
            return NotFound("Coworking center not found!");

        var ccRevenue = CreateCenterRevenueReport(center, timeBackDateTime);

        return Ok(new CoworkingCenterRevenueResponseDto
        {
            TimeBack = timeBack,
            Revenue = ccRevenue
        });
    }

    private WorkspaceRevenueDto CreateRevenueReport(Workspace workspace, DateTime timeBack)
    {
        var finishedReservations = workspace.Reservations.Where(r => r.EndTime >= timeBack && r.EndTime <= DateTime.UtcNow);

        return new WorkspaceRevenueDto
        {
            WorkspaceId = workspace.WorkspaceId,
            WorkspaceDisplayName = workspace.Name,
            CoworkingCenterId = workspace.CoworkingCenterId,
            Revenue = finishedReservations.Sum(r => r.TotalPrice).GetValueOrDefault(),
            FinishedReservations = finishedReservations.Select(f => f.ReservationId).ToList(),
        };
    }

    private CoworkingCenterRevenueDto CreateCenterRevenueReport(CoworkingCenter cc, DateTime timeBackDateTime)
    {
        var revenues = cc.Workspaces.Select(w => CreateRevenueReport(w, timeBackDateTime)).ToList();

        return new CoworkingCenterRevenueDto
        {
            CoworkingCenterId = cc.CoworkingCenterId,
            CoworkingCenterDisplayName = cc.Name,
            Revenue = cc.Workspaces.Sum(w =>
                w.Reservations
                    .Where(r => r.EndTime >= timeBackDateTime && r.EndTime <= DateTime.UtcNow)
                    .Sum(r => r.TotalPrice)
                    .GetValueOrDefault()),
            Revenues = revenues
        };
    }
}
