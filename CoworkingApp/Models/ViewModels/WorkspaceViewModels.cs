using CoworkingApp.Models.DataModels;
using CoworkingApp.Models.DtoModels;
using CoworkingApp.Models.Misc;
using CoworkingApp.Services.Repositories;

namespace CoworkingApp.Models.ViewModels;

public class WorkspaceIndexViewModel
{
    public IEnumerable<Workspace> Workspaces { get; set; } = [];
    public PaginationRequestDto Pagination { get; set; } = null!;
    public required WorkspaceSort Sort { get; set; }
}

public class WorkspaceDetailViewModel : TimelineData
{
    public required IEnumerable<WorkspaceHistory> Histories { get; set; }
    public required WorkspaceHistory? LatestWorkspaceHistory { get; set; }
    public required CoworkingCenter CoworkingCenter { get; set; }
}

public class WorkspaceReserveViewModel : TimelineData
{
    public required ReservationCreateRequestDto Request { get; set; }
}