using CoworkingApp.Models.DTOModels.User;
using CoworkingApp.Models.DTOModels.Workspace;
using CoworkingApp.Services;

namespace CoworkingApp.Models.DTOModels.Reservation;

/////////////////////////////////////////////////////////////
// Data DTOs
/////////////////////////////////////////////////////////////


public class ReservationDto
{
    public int Id { get; set; }
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

public class ReservationQueryRequestDto : PaginationRequestDto
{
    public int? WorkspaceId { get; set; }
    public int? CustomerId { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public NullableRangeFilter<decimal> Price { get; set; } = new();
    public int? PricingId { get; set; }
}

public class AdminReservationQueryRequestDto : ReservationQueryRequestDto
{
    
}

public class ReservationCreateRequestDto
{
    public int WorkspaceId { get; init; }
    public DateTime StartTime { get; set; } = DateTime.Now.AddMinutes(10);
    public DateTime EndTime { get; init; } = DateTime.Now.AddHours(1);
}

[AdminDto]
public class ReservationUpdateRequestDto
{
    public int? WorkspaceId { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public bool? IsCancelled { get; set; }
}

/////////////////////////////////////////////////////////////
// Response DTOs
/////////////////////////////////////////////////////////////

public class ReservationsResponseDto : PaginationResponseDto
{
    public required IEnumerable<ReservationDto> Reservations { get; set; }
}