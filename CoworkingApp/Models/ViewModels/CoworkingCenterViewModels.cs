using CoworkingApp.Models.DataModels;
using CoworkingApp.Models.DtoModels;

namespace CoworkingApp.Models.ViewModels;

public class CoworkingCenterIndexViewModel
{
    public required IEnumerable<CoworkingCenter> CoworkingCenters { get; set; } = [];
    public required PaginationRequestDto Pagination { get; set; } = null!;
    public required int TotalCount { get; set; }
}