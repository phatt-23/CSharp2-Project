using CoworkingApp.Models.DTOModels.CoworkingCenters;
using CoworkingApp.Models.DTOModels.Reservation;
using CoworkingApp.Models.DTOModels.WorkspaceStatus;

namespace CoworkingApp.Models.DTOModels.Workspace;

/////////////////////////////////////
// DTOs with data
/////////////////////////////////////
 
public class WorkspaceDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public CoworkingCenterDto CoworkingCenter { get; set; } = null!;
}

public class WorkspaceDetailDto : WorkspaceDto
{
}

[AdminDto]
public class AdminWorkspaceDto : WorkspaceDetailDto
{
    public bool IsRemoved { get; init; }
    public int CoworkingCenterId { get; init; }
    public DateTime CreatedAt { get; init; }
    public WorkspaceStatusDto Status { get; set; } = null!;
}


[AdminDto]
public class WorkspaceHistoryDto
{
    public int Id { get; set; }
    public int WorkspaceId { get; set; }
    public int StatusId { get; set; }
    public DateTime CreatedAt { get; set; }
}

[AdminDto]
public class WorkspacePricingDto
{
    public int Id { get; set; }
    public int WorkspaceId { get; set; }
    public decimal PricePerHour { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime? ValidUntil { get; set; }
}

