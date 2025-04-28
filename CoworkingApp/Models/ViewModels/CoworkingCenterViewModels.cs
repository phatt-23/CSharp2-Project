using CoworkingApp.Models.DataModels;
using CoworkingApp.Models.DtoModels;

namespace CoworkingApp.Models.ViewModels;

public class CoworkingCenterIndexViewModel
{
    public IEnumerable<CoworkingCenter> CoworkingCenters { get; set; } = [];
    public PaginationRequestDto Pagination { get; set; } = null!;
}