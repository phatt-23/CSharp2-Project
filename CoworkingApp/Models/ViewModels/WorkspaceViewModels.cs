using CoworkingApp.Models.DataModels;
using CoworkingApp.Models.DtoModels;

namespace CoworkingApp.Models.ViewModels;

public class WorkspaceIndexViewModel
{
    public IEnumerable<Workspace> Workspaces { get; set; } = [];
    public PaginationRequestDto Pagination { get; set; } = null!;
}

public class WorkspaceDetailViewModel 
{
    public Workspace Workspace { get; set; } = null!;
    public IEnumerable<WorkspaceHistory> Histories { get; set; } = null!;
    public IEnumerable<Reservation> Reservations { get; set; } = null!; 
    public CoworkingCenter CoworkingCenter { get; set; } = null!;
}

public class WorkspaceReserveViewModel 
{
    public Workspace Workspace { get; set; } = null!;
    public ReservationCreateRequestDto Request { get; set; } = null!;
    public IEnumerable<Reservation> Reservations { get; set; } = null!;
}