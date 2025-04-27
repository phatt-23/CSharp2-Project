using CoworkingApp.Models.DTOModels.Workspace;
using CoworkingApp.Services;

namespace CoworkingApp.Models.DTOModels.CoworkingCenters;

/////////////////////////////////////////////////////////////////////////////////
// Data DTOs
/////////////////////////////////////////////////////////////////////////////////

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


/////////////////////////////////////////////////////////////////////////////////
// Request DTOs
/////////////////////////////////////////////////////////////////////////////////


public sealed class CoworkingCenterQueryRequestDto : PaginationRequestDto 
{
    public string? LikeName { get; set; }
    public RangeFilter<decimal> Latitude { get; set; } = new();
    public RangeFilter<decimal> Longitude { get; set; } = new();
}


[AdminDto]
public class CoworkingCenterCreateRequestDto
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
}

[AdminDto]
public class CoworkingCenterUpdateRequestDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
}

/////////////////////////////////////////////////////////////////////////////////
// Response DTOs
/////////////////////////////////////////////////////////////////////////////////

public class CoworkingCentersResponseDto : PaginationResponseDto
{
    public required IEnumerable<CoworkingCenterDto> Centers { get; set; }
}
