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
    public DateTime LastUpdated { get; set; } // Auditing
    public int? UpdatedBy { get; set; } // Auditing
}

/////////////////////////////////////////////////////////////////////////////////
// Request DTOs
/////////////////////////////////////////////////////////////////////////////////

[PublicRequestDto]
public sealed class CoworkingCenterQueryRequestDto : PaginationRequestDto 
{
    [Required] public string? NameContains { get; set; }
    [Required] public NullableRangeFilter<decimal> Latitude { get; set; } = new();
    [Required] public NullableRangeFilter<decimal> Longitude { get; set; } = new();
}

[AdminRequestDto]
public class CoworkingCenterCreateRequestDto
{
    [Required] public string Name { get; set; } = null!;
    [Required] public string Description { get; set; } = string.Empty;
    [Required] public decimal Latitude { get; set; }
    [Required] public decimal Longitude { get; set; }
}

[AdminRequestDto]
public class CoworkingCenterCreateWithAddressRequestDto
{
    [Required] public required string Name { get; set; }
    [Required] public required string Description { get; set; }
    [Required] public string StreetAddress { get; set; } = null!;
    [Required] public string District { get; set; } = null!;
    [Required] public string City { get; set; } = null!;
    [Required] public string PostalCode { get; set; } = null!;
    [Required] public string Country { get; set; } = null!;
}

[AdminRequestDto]
public class CoworkingCenterUpdateRequestDto
{
    [Required] public required int CoworkingCenterId { get; set;}
    [Required] public required string Name { get; set; }
    [Required] public required string Description { get; set; }
    [Required] public required decimal Latitude { get; set; }
    [Required] public required decimal Longitude { get; set; }
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
