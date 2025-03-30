using CoworkingApp.Models.DTOModels.Workspace;

namespace CoworkingApp.Models.DTOModels.CoworkingCenters;

// Data DTOs

public class CoworkingCenterDto
{
    public int Id { get; set; }
    public required string Name { get; set; } 
    public required string Description { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
}


[AdminDto]
public class AdminCoworkingCenterDto : CoworkingCenterDto
{
    public DateTime CreatedAt { get; set; } 
    public virtual IEnumerable<AdminWorkspaceDto> Workspaces { get; set; } = [];
}