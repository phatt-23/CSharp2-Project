using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using CoworkingApp.Models.DataModels;
using CoworkingApp.Models.DTOModels.CoworkingCenters;
using CoworkingApp.Models.DTOModels.Reservation;
using CoworkingApp.Models.DTOModels.WorkspaceStatus;

namespace CoworkingApp.Models.DTOModels.Workspace;

/////////////////////////////////////
// Data DTOs
/////////////////////////////////////
 
public class WorkspaceDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public LatestWorkspacePricingDto? LatestPricing { get; set; }
    public int CoworkingCenterId { get; set; }
}

public class WorkspaceDetailDto : WorkspaceDto
{
    public IEnumerable<WorkspacePricingDto> WorkspacePricings { get; set; } = [];
    public CoworkingCenterDto CoworkingCenter { get; set; } = null!;
}

[AdminDto]
public class AdminWorkspaceDto : WorkspaceDto
{
    public bool IsRemoved { get; init; }
    public DateTime CreatedAt { get; init; }
    public WorkspaceStatusDto Status { get; set; } = null!;
}

public class AdminWorkspaceDetailDto :  AdminWorkspaceDto
{
    public IEnumerable<AdminWorkspacePricingDto> WorkspacePricings { get; set; } = [];
    public AdminCoworkingCenterDto CoworkingCenter { get; set; } = null!;
}

//////////////////////////////////////////////////////////////
// Request DTOs
//////////////////////////////////////////////////////////////

public class WorkspaceQueryRequestDto : PaginationRequestDto
{
    public string? LikeName { get; set; }
}


[AdminDto]
public class AdminWorkspaceQueryRequestDto : WorkspaceQueryRequestDto
{
    // Only admin can filter by status, because users only see available ones
    public int? StatusId { get; set; }
    public WorkspaceStatusType? StatusType { get; set; }
}

[AdminDto]
public class WorkspaceCreateRequestDto
{
    [Required] public string Name { get; set; } = null!;
    [Required] public string Description { get; set; } = null!;
    [Required] public int CoworkingCenterId { get; set; }
    [Required] public int StatusId { get; set; }
}

[AdminDto]
public class WorkspaceUpdateRequestDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int? CoworkingCenterId { get; set; }
}


////////////////////////////////////////////////////////////
// Response DTOs
////////////////////////////////////////////////////////////

 
public class WorkspacesResponseDto : PaginationResponseDto
{
    public required IEnumerable<WorkspaceDto> Workspaces { get; set; }
}


[AdminDto]
public class AdminWorkspacesResponseDTo : PaginationResponseDto
{
    public required IEnumerable<AdminWorkspaceDto> Workspaces { get; set; }
    public WorkspacePricingDto? LatestPricing { get; set; }
}


[AdminDto]
public class WorkspaceHistoriesResponseDto
{
    public required WorkspaceDto Workspace { get; set; }
    public required IEnumerable<WorkspaceHistoryDto> Histories { get; set; }
}


