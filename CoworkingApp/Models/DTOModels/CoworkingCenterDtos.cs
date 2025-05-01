using CoworkingApp.Services;
using System.ComponentModel.DataAnnotations;

namespace CoworkingApp.Models.DtoModels;

/////////////////////////////////////////////////////////////////////////////////
// Data DTOs (response)
/////////////////////////////////////////////////////////////////////////////////

[PublicDataDto]
public class CoworkingCenterDto
{
    public int CoworkingCenterId { get; set; }
    public required string Name { get; set; } 
    public required string Description { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public string AddressDisplayName { get; set; } = string.Empty; // Computed flattened address
}


[AdminDataDto]
public class AdminCoworkingCenterDto
{
    public int CoworkingCenterId { get; set; } // Required for CRUD
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public int AddressId { get; set; } // Id for relate data
    public string AddressDisplayName { get; set; } = null!; // Flattened address for display
    public int WorkspaceCount { get; set; } // Summary of related data
    public DateTime LastUpdated { get; set; } // Auditing
    public int? UpdatedBy { get; set; } // Auditing
}

/////////////////////////////////////////////////////////////////////////////////
// Request DTOs
/////////////////////////////////////////////////////////////////////////////////

[PublicRequestDto]
public sealed class CoworkingCenterQueryRequestDto : PaginationRequestDto 
{
    public string? NameContains { get; set; }
    public NullableRangeFilter<decimal> Latitude { get; set; } = new();
    public NullableRangeFilter<decimal> Longitude { get; set; } = new();
}

[AdminRequestDto]
public class CoworkingCenterCreateRequestDto
{
    [Required] public string Name { get; set; } = null!;
    public string Description { get; set; } = string.Empty;
    [Required] public int Latitude { get; set; }
    [Required] public int Longitude { get; set; }
}

[AdminRequestDto]
public class CoworkingCenterCreateWithAddressRequestDto
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string StreetAddress { get; set; } = null!;
    public string District { get; set; } = null!;
    public string City { get; set; } = null!;
    public string PostalCode { get; set; } = null!;
    public string Country { get; set; } = null!;
}

[AdminRequestDto]
public class CoworkingCenterUpdateRequestDto
{
    [Required] public required string Name { get; set; }
    [Required] public required string Description { get; set; } 
    [Required] public required string StreetAddress { get; set; } 
    [Required] public required string District { get; set; } 
    [Required] public required string City { get; set; } 
    [Required] public required string PostalCode { get; set; }
    [Required] public required string Country { get; set; } 
}

/////////////////////////////////////////////////////////////////////////////////
// Response DTOs
/////////////////////////////////////////////////////////////////////////////////

[PublicResponseDto]
public class CoworkingCenterQueryResponseDto : PaginationResponseDto
{
    public required IEnumerable<CoworkingCenterDto> Centers { get; set; }
}

[AdminResponseDto]
public class AdminCoworkingCenterQueryResponseDto : PaginationResponseDto
{
    public required IEnumerable<AdminCoworkingCenterDto> Centers { get; set; }
}
