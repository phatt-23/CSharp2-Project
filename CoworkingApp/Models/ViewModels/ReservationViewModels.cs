using CoworkingApp.Models.DataModels;
using CoworkingApp.Models.DtoModels;

namespace CoworkingApp.Models.ViewModels;

public class ReservationCreateGetViewModel
{
    public IEnumerable<Workspace> Workspaces { get; set; } = null!;
    public ReservationCreateRequestDto Request { get; set; } = null!;
}

public class ReservationDetailViewModel
{
    public required Reservation Reservation { get; set; }
    public required Workspace Workspace { get; set; }
}