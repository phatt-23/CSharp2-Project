using System.Text.Json.Serialization;
using CoworkingApp.Models.DTOModels.Reservation;
using CoworkingApp.Models.DTOModels.Workspace;
using CoworkingApp.Services;

//////////////////////////////////////////
// Data DTOs
//////////////////////////////////////////

public class WorkspacePricingDto
{
    public int Id { get; set; }
    public int WorkspaceId { get; set; }
    public decimal PricePerHour { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime? ValidUntil { get; set; }
}

// Lighter => for workspace dto responses, no cyclic references
public class LatestWorkspacePricingDto
{
    public int Id { get; set; }
    public int WorkspaceId { get; set; }
    public decimal PricePerHour { get; set; }
    public DateTime ValidFrom { get; set; }
}

public class AdminWorkspacePricingDto : WorkspacePricingDto
{
    public virtual ICollection<ReservationDto> Reservations { get; set; } = [];
    public virtual WorkspaceDto Workspace { get; set; } = null!;
}


//////////////////////////////////////////
// Request DTOs
//////////////////////////////////////////

public class WorkspacePricingQueryRequestDto
{
    public int? WorkspaceId { get; set; }
    public RangeFilter<decimal> PricePerHour { get; set; } = new();
    public DateTime ValidFrom { get; set; }
    public DateTime? ValidUntil { get; set; }
    public bool IncludeReservations { get; set; } = false;
    public bool IncludeWorkspace { get; set; } = false;
}


public class WorkspacePricingCreateRequestDto
{
    public int WorkspaceId { get; set; }
    public decimal PricePerHour { get; set; }
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidUntil { get; set; }
}