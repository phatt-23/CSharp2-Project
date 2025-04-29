using CoworkingApp.Models.DataModels;
using CoworkingApp.Models.DtoModels;
using CoworkingApp.Models.Misc;

namespace CoworkingApp.Models.ViewModels;

public class ReservationCreateGetViewModel
{
    public required IEnumerable<Workspace> Workspaces { get; set; }
    public required ReservationCreateRequestDto Request { get; set; } 
}

public class ReservationDetailViewModel
{
    public required Reservation Reservation { get; set; }
    public required Workspace Workspace { get; set; }
}

public class ReservationEditViewModel : TimelineData
{
    public required WorkspaceHistory? LatestWorkspaceHistory { get; set; }
    public required ReservationUpdateRequestDto Request { get; set; } = null!;
}