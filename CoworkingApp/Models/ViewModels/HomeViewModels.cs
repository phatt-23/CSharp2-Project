using CoworkingApp.Models.DataModels;
using CoworkingApp.Services.Repositories;

namespace CoworkingApp.Models.ViewModels;

public class HomeIndexViewModel
{
    public IEnumerable<Workspace> Workspaces { get; set; } = [];
    public IEnumerable<CoworkingCenter> CoworkingCenters { get; set; } = [];
}

public class HomeDashboardViewModel
{
    public required IEnumerable<Reservation> Reservations { get; set; }
    public required User User { get; set; }
    public required ReservationSort ReservationSort { get; set; }
}