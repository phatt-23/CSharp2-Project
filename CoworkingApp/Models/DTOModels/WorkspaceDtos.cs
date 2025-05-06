using System.ComponentModel.DataAnnotations;
using AutoFilterer.Enums;
using CoworkingApp.Models.DataModels;
using CoworkingApp.Services;
using CoworkingApp.Services.Repositories;

namespace CoworkingApp.Models.DtoModels;

/////////////////////////////////////
// Data DTOs
/////////////////////////////////////

[PublicDataDto]
public class WorkspaceDto
{
    public required int WorkspaceId { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required decimal PricePerHour { get; set; }
    public required int CoworkingCenterId { get; set; }
    public required string CoworkingCenterDisplayName { get; set; }
    public required WorkspaceStatusType Status { get; set; }
}

[PublicDataDto]
public class WorkspaceDetailDto : WorkspaceDto
{
    public required IEnumerable<WorkspacePricingDto> WorkspacePricings { get; set; }
    public required CoworkingCenterDto CoworkingCenter { get; set; }
}

[AdminDataDto]
public class AdminWorkspaceDto : WorkspaceDto
{
    public bool IsRemoved { get; set; }
    public DateTime LastUpdated { get; set; }
}

[AdminDataDto]
public class AdminWorkspaceDetailDto :  AdminWorkspaceDto
{
    public required IEnumerable<AdminWorkspacePricingDto> WorkspacePricings { get; set; }
    public required AdminCoworkingCenterDto CoworkingCenter { get; set; }
}

//////////////////////////////////////////////////////////////
// Request DTOs
//////////////////////////////////////////////////////////////

[PublicRequestDto]
public class WorkspaceQueryRequestDto : PaginationRequestDto
{
    public string? NameContains { get; set; }
    public NullableRangeFilter<decimal> PricePerHour { get; set; } = new();
    public WorkspaceStatusType? Status { get; set; }
    public int? CoworkingCenterId { get; set; }
}

[AdminRequestDto]
public class AdminWorkspaceQueryRequestDto : WorkspaceQueryRequestDto
{
}

[AdminRequestDto]
public class WorkspaceCreateRequestDto
{
    [Required] public string Name { get; set; } = null!;
    [Required] public string Description { get; set; } = null!;
    [Required] public int CoworkingCenterId { get; set; }
    [Required] public decimal PricePerHour { get; set; }
    [Required] public WorkspaceStatusType Status { get; set; }
}

[AdminRequestDto]
public class WorkspaceUpdateRequestDto
{
    [Required] public required int WorkspaceId { get; set; }
    [Required] public required string Name { get; set; }
    [Required] public required string Description { get; set; }
    [Required] public required int CoworkingCenterId { get; set; }
    [Required] public decimal PricePerHour { get; set; }
    [Required] public required WorkspaceStatusType Status { get; set; }
}

[PublicRequestDto]
public class WorkspaceSortRequestDto
{
    public WorkspaceSort Sort;
}

////////////////////////////////////////////////////////////
// Response DTOs
////////////////////////////////////////////////////////////

[PublicResponseDto]
public class WorkspacesResponseDto : PaginationResponseDto
{
    public required IEnumerable<WorkspaceDto> Workspaces { get; set; }
}

[AdminResponseDto]
public class AdminWorkspacesResponseDto : PaginationResponseDto
{
    public required IEnumerable<AdminWorkspaceDto> Workspaces { get; set; }
}

[AdminResponseDto]
public class WorkspaceHistoriesResponseDto : PaginationResponseDto
{
    public required IEnumerable<WorkspaceHistoryDto> Histories { get; set; }
}


// this is just for when a user wants to see where are the free times
// they can make a reservation. exposing just enough information to
// be able to figure out time when they can make a reservation
[PublicDataDto]
public class AnonymousReservationDto
{
    public required DateTime StartTime { get; set; }
    public required DateTime EndTime { get; set; }
}

[PublicResponseDto]
public class WorkspaceReservationsResponseDto
{
    public required WorkspaceDto Workspace { get; set; }
    public required ICollection<AnonymousReservationDto> Reservations { get; set; }
}


public class AdminWorkspacePricingsResponseDto : PaginationResponseDto
{
    public required List<AdminWorkspacePricingDto> Pricings { get; set; }
}

public class AdminWorkspaceStatusHistoryResponseDto : PaginationResponseDto
{
    public required List<WorkspaceStatusDto> Statuses { get; set; }
}