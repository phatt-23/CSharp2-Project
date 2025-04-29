using CoworkingApp.Services;

namespace CoworkingApp.Models.DtoModels;

/////////////////////////////////////////////////////////////
// Data DTOs
/////////////////////////////////////////////////////////////

[PublicDto]
public class ReservationDto
{
    public int ReservationId { get; set; }
    public int WorkspaceId { get; set; }
    public int CustomerId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public decimal TotalPrice { get; set; }
    public int? PricingId { get; set; }
}

[AdminDto]
public class AdminReservationDto : ReservationDto
{
    public bool IsCancelled { get; set; }
    
    public virtual UserDto Customer { get; set; } = null!;
    
    public virtual WorkspacePricingDto? Pricing { get; set; }

    public virtual AdminWorkspaceDto Workspace { get; set; } = null!;
}

/////////////////////////////////////////////////////////////
// Request DTOs
/////////////////////////////////////////////////////////////

// READ

[PublicDto]
public class ReservationQueryRequestDto : PaginationRequestDto
{
    public int? WorkspaceId { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public NullableRangeFilter<decimal> Price { get; set; } = new();
}

[AdminDto]
public class AdminReservationQueryRequestDto : ReservationQueryRequestDto
{
    public int? WorkspaceId { get; set; }
    public int? CustomerId { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public NullableRangeFilter<decimal> Price { get; set; } = new();
    public int? PricingId { get; set; }
}

// CREATE

[PublicDto]
public class ReservationCreateRequestDto
{
    public int WorkspaceId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}

// UPDATE

[PublicDto]
public class ReservationUpdateRequestDto
{
    public required int ReservationId { get; set; }
    public required DateTime StartTime { get; set; }
    public required DateTime EndTime { get; set; }
}

[AdminDto]
public class AdminReservationUpdateRequestDto
{
    public required int ReservationId { get; set; }
    public required DateTime StartTime { get; set; }
    public required DateTime EndTime { get; set; }
    public required bool IsCancelled { get; set; }
}

/////////////////////////////////////////////////////////////
// Response DTOs
/////////////////////////////////////////////////////////////

public class ReservationsResponseDto : PaginationResponseDto
{
    public required IEnumerable<ReservationDto> Reservations { get; set; }
}