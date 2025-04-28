using CoworkingApp.Services;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using CoworkingApp.Models.DataModels;

namespace CoworkingApp.Models.DtoModels;

/////////////////////////////////////////////////////////////////////////////////
// Data DTOs (response)
/////////////////////////////////////////////////////////////////////////////////

[PublicDto]
public class CoworkingCenterDto
{
    public int CoworkingCenterId { get; set; }
    public required string Name { get; set; } 
    public required string Description { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public string AddressDisplayName { get; set; } = string.Empty; // Computed flattened address
}

[AdminDto]
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

[PublicDto]
public sealed class CoworkingCenterQueryRequestDto : PaginationRequestDto 
{
    public string? NameContains { get; set; }
    public RangeFilter<decimal> Latitude { get; set; } = new();
    public RangeFilter<decimal> Longitude { get; set; } = new();
}

[AdminDto]
public class CoworkingCenterCreateRequestDto
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string StreetAddress { get; set; } = null!;
    public string District { get; set; } = null!;
    public string City { get; set; } = null!;
    public string PostalCode { get; set; } = null!;
    public string Country { get; set; } = null!;
}

[AdminDto]
public class CoworkingCenterUpdateRequestDto
{
    [Required] public string Name { get; set; }
    [Required] public string Description { get; set; } 
    [Required] public string StreetAddress { get; set; } 
    [Required] public string District { get; set; } 
    [Required] public string City { get; set; } 
    [Required] public string PostalCode { get; set; }
    [Required] public string Country { get; set; } 
}

/////////////////////////////////////////////////////////////////////////////////
// Response DTOs
/////////////////////////////////////////////////////////////////////////////////

[PublicDto]
public class CoworkingCenterQueryResponseDto : PaginationResponseDto
{
    public required IEnumerable<CoworkingCenterDto> Centers { get; set; }
}

[AdminDto]
public class AdminCoworkingCenterQueryResponseDto : PaginationResponseDto
{
    public required IEnumerable<AdminCoworkingCenterDto> Centers { get; set; }
}
