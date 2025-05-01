using CoworkingApp.Data;
using CoworkingApp.Models.DtoModels;
using CoworkingApp.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoworkingApp.Controllers.ApiEndpointContollers.Admin;

[AdminApiController]
[Authorize(Roles = "Admin")]
[Route("/api/admin/status")]
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

        var reveues = await db.Workspaces
            .Include(w => w.Reservations)
            .Select(w => new WorkspaceRevenueDto
            {
                WorkspaceId = w.WorkspaceId,
                CoworkingCenterId = w.CoworkingCenterId,
                Revenue = w.Reservations
                    .Where(r => r.EndTime >= timeBackDateTime)
                    .Sum(r => r.TotalPrice)
                    .GetValueOrDefault(),
            })
            .ToListAsync();

        return Ok(new WorkspaceRevenuesResponseDto
        {
            TimeBack = timeBack,
            Revenues = reveues,
        });
    }

    [HttpGet("coworking-center/revenue")]
    public async Task<ActionResult<WorkspaceRevenuesResponseDto>> GetCoworkingCenterRevenues([FromQuery] TimeBack timeBack = TimeBack.LastMonth)
    {
        var timeBackDateTime = timeBack.ToDateTime();

        var reveues = await db.CoworkingCenters
            .Include(cc => cc.Workspaces)
            .ThenInclude(w => w.Reservations)
            .Select(cc => new CoworkingCenterRevenueDto
            {
                CoworkingCenterId = cc.CoworkingCenterId,
                Revenue = cc.Workspaces.Sum(w =>
                    w.Reservations
                        .Where(r => r.EndTime >= timeBackDateTime)
                        .Sum(r => r.TotalPrice)
                        .GetValueOrDefault()
                )
            })
            .ToListAsync();

        return Ok(new CoworkingCenterRevenuesResponseDto
        {
            TimeBack = timeBack,
            Revenues = reveues,
        });
    }
}
