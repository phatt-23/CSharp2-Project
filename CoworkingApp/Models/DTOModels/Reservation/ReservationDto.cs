namespace CoworkingApp.Models.DTOModels.Reservation;

public class ReservationDto
{
    public int Id { get; set; }
    public int WorkspaceId { get; set; }
    public int CustomerId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public decimal TotalPrice { get; set; }
    public int? PricingId { get; set; }
    public bool IsCancelled { get; set; }
}