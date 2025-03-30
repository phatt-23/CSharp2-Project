namespace CoworkingApp.Models.DTOModels.Reservation;

// Request DTOs

public class ReservationQueryRequestDto : PaginationRequestDto
{
    public int? WorkspaceId { get; set; }
    public int? CustomerId { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public decimal? PriceLow { get; set; }
    public decimal? PriceHigh { get; set; }
    public int? PricingId { get; set; }
}


public class ReservationCreateRequestDto
{
    public int WorkspaceId { get; set; }
    public int CustomerId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}
