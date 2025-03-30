namespace CoworkingApp.Models.DTOModels.Reservation;

public class ReservationsQueryDto : PaginationQueryDto
{
    public int? WorkspaceId { get; set; }
    public int? CustomerId { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public decimal? TotalPrice { get; set; }
    public decimal? PriceLow { get; set; }
    public decimal? PriceHigh { get; set; }
    public int? PricingId { get; set; }
}