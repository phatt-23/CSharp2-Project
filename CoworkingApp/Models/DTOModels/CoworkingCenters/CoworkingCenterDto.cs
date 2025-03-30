namespace CoworkingApp.Models.DTOModels.CoworkingCenters;

public class CoworkingCenterDto
{
    public int Id { get; set; }
    
    public required string Name { get; set; } 

    public required string Description { get; set; }

    public decimal Latitude { get; set; }

    public decimal Longitude { get; set; }
}