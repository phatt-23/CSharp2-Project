using CoworkingApp.Services;
using System.ComponentModel.DataAnnotations;

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
    public int PricingId { get; set; }
    public string WorkspaceDisplayName { get; set; } = null!;
    public decimal PricingPerHour { get; set; }
}

[AdminDto]
public class AdminReservationDto : ReservationDto
{
    public string CustomerEmail { get; set; } = null!;
    public bool IsCancelled { get; set; }
    public DateTime CreatedAt { get; set; }
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

[PublicRequestDto]
public class ReservationCreateRequestDto
{
    [Required]
    public int WorkspaceId { get; set; }
    [Required]
    public DateTime StartTime { get; set; }
    [Required]
    public DateTime EndTime { get; set; }
}

[AdminRequestDto]
public class AdminReservationCreateDto : ReservationCreateRequestDto
{
    [Required]
    public int CustomerId { get; set; }
}

// UPDATE

[PublicRequestDto]
public class ReservationUpdateRequestDto
{
    [Required] public required int ReservationId { get; set; }
    [Required] public required int WorkspaceId { get; set; }
    [Required] public required DateTime StartTime { get; set; }
    [Required] public required DateTime EndTime { get; set; }
}

[AdminRequestDto]
public class AdminReservationUpdateRequestDto : ReservationUpdateRequestDto
{
    [Required] public required int CustomerId { get; set; }
}

/////////////////////////////////////////////////////////////
// Response DTOs
/////////////////////////////////////////////////////////////

public class ReservationsResponseDto : PaginationResponseDto
{
    public required IEnumerable<ReservationDto> Reservations { get; set; }
}

[AdminResponseDto]
public class AdminReservationsResponseDto : PaginationResponseDto
{
    public required List<AdminReservationDto> Reservations { get; set; }
}