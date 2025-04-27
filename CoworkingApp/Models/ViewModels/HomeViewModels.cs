using CoworkingApp.Models.DataModels;

namespace CoworkingApp.Models.ViewModels;

public class HomeIndexViewModel
{
    public IEnumerable<Workspace> Workspaces { get; set; } = [];
    public IEnumerable<CoworkingCenter> CoworkingCenters { get; set; } = [];
}