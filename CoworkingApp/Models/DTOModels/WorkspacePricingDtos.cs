using CoworkingApp.Services;
using System.ComponentModel.DataAnnotations;

namespace CoworkingApp.Models.DtoModels;

//////////////////////////////////////////
// Data DTOs
//////////////////////////////////////////

[PublicDataDto]
public class WorkspacePricingDto
{
    public int Id { get; set; }
    public int WorkspaceId { get; set; }
    public decimal PricePerHour { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime? ValidUntil { get; set; }
}

[AdminDataDto]
public class AdminWorkspacePricingDto : WorkspacePricingDto
{
}


//////////////////////////////////////////
// Request DTOs
//////////////////////////////////////////

[PublicRequestDto]
public class WorkspacePricingQueryRequestDto
{
    public int? WorkspaceId { get; set; }
    public RangeFilter<decimal> PricePerHour { get; set; } = new();
    public DateTime ValidFrom { get; set; }
    public DateTime? ValidUntil { get; set; }
    public bool IncludeReservations { get; set; } = false;
    public bool IncludeWorkspace { get; set; } = false;
}

[AdminRequestDto]
public class WorkspacePricingCreateRequestDto
{
    [Required]
    public int WorkspaceId { get; set; }
    [Required]
    public decimal PricePerHour { get; set; }
    [Required]
    public DateTime ValidFrom { get; set; }
    public DateTime? ValidUntil { get; set; }
}