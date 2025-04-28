using CoworkingApp.Models.DataModels;

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
}