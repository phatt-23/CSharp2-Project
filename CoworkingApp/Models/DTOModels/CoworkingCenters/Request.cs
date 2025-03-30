namespace CoworkingApp.Models.DTOModels.CoworkingCenters;

// Request DTOs

public sealed class CoworkingCenterQueryRequestDto : PaginationRequestDto 
{
    public string? Name { get; set; } 
    
    public decimal? LatitudeLow { get; set; }
    public decimal? LatitudeHigh { get; set; }
    
    public decimal? LongitudeLow { get; set; }
    public decimal? LongitudeHigh { get; set; }
}


[AdminDto]
public class CoworkingCenterCreateRequestDto
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
}
