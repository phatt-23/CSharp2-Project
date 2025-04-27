using CoworkingApp.Models.DataModels;
using CoworkingApp.Models.DTOModels.Reservation;

namespace CoworkingApp.Models.ViewModels;

public class ReservationCreateGetViewModel
{
    public IEnumerable<Workspace> Workspaces { get; set; } = null!;
    public ReservationCreateRequestDto Request { get; set; } = null!;
}