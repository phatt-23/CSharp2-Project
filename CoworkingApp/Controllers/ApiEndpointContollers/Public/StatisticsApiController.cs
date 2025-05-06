using CoworkingApp.Data;
using CoworkingApp.Models.DtoModels;
using CoworkingApp.Types;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace CoworkingApp.Controllers.ApiEndpointContollers.Public;

public interface IStatisticsApi
{
}

[PublicApiController]
[Route("/api/stats")]
public class StatisticsApiController
    (
        CoworkingDbContext db
    )
    : Controller, IStatisticsApi
{
    [HttpGet("workspace/reservation-count")]
    public async Task<ActionResult<WorkspaceReservationCountsResponseDto>> GetWorkspaceReservationCount([FromQuery] TimeBack timeBack = TimeBack.LastMonth)
    {
        var timeBackDateTime = timeBack.ToDateTime();

        var workspaceReservationCounts = await db.Workspaces
            .Include(w => w.Reservations)
            .Select(w => new WorkspaceReservationCountDto
            {
                WorkspaceId = w.WorkspaceId,
                CoworkingCenterId = w.CoworkingCenterId,
                ReservationCount = w.Reservations.Count(r => r.EndTime >= timeBackDateTime && r.EndTime <= DateTime.UtcNow),
            })
            .ToListAsync();

        return Ok(new WorkspaceReservationCountsResponseDto
        {
            TimeBack = timeBack,
            ReservationCounts = workspaceReservationCounts
        });
    }

    [HttpGet("coworking-center/reservation-count")]
    public async Task<ActionResult<CoworkingCenterReservationCountsResponseDto>> GetCoworkingCenterReservationCount([FromQuery] TimeBack timeBack = TimeBack.LastMonth)
    {
        var timeBackDateTime = timeBack.ToDateTime();

        var coworkingCenterReservationCounts = await db.CoworkingCenters
            .Include(cc => cc.Workspaces)
            .ThenInclude(w => w.Reservations)
            .Select(cc => new CoworkingCenterReservationCountDto
            {
                CoworkingCenterId = cc.CoworkingCenterId,
                ReservationCount = cc.Workspaces.Sum(w => w.Reservations.Count(r => r.EndTime >= timeBackDateTime && r.EndTime <= DateTime.UtcNow)),
            })
            .ToListAsync();

        return Ok(new CoworkingCenterReservationCountsResponseDto
        {
            TimeBack = timeBack,
            ReservationCounts = coworkingCenterReservationCounts,
        });
    }
}
