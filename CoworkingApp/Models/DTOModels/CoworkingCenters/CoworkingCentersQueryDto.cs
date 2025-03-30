namespace CoworkingApp.Models.DTOModels.CoworkingCenters;

public sealed class CoworkingCentersQueryDto : PaginationQueryDto 
{
    public string? Name { get; set; } 
    
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    
    public decimal? LatitudeLow { get; set; }
    public decimal? LatitudeHigh { get; set; }
    
    public decimal? LongitudeLow { get; set; }
    public decimal? LongitudeHigh { get; set; }
}