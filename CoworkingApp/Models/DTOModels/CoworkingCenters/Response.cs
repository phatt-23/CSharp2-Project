namespace CoworkingApp.Models.DTOModels.CoworkingCenters;

public class CoworkingCentersResponseDto : PaginationResponseDto
{
    public required IEnumerable<CoworkingCenterDto> Centers { get; set; }
}