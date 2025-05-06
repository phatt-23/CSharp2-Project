using CoworkingApp.Models.DataModels;
using CoworkingApp.Models.DtoModels;
using CoworkingApp.Models.Misc;
using CoworkingApp.Services.Repositories;

namespace CoworkingApp.Models.ViewModels;

public class WorkspaceIndexViewModel
{
    public IEnumerable<Workspace> Workspaces { get; set; } = [];
    public PaginationRequestDto Pagination { get; set; } = null!;
    public int TotalCount { get; set; }
    public required WorkspaceSort Sort { get; set; }
}

public class WorkspaceDetailViewModel
{
    public required IEnumerable<WorkspaceHistory> Histories { get; set; }
    public required WorkspaceHistory? LatestWorkspaceHistory { get; set; }
    public required CoworkingCenter CoworkingCenter { get; set; }
    public required TimelineData Timeline { get; set; }
    public decimal PricePerHour { get; set; }

}

public class WorkspaceReserveViewModel
{
    public required Workspace Workspace { get; set; }
    public required ReservationCreateRequestDto Request { get; set; }
    public required TimelineData Timeline { get; set; }
}
