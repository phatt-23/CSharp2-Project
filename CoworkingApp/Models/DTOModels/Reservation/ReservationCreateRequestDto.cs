namespace CoworkingApp.Models.DTOModels.Reservation;

public class ReservationCreateRequestDto
{
    public int WorkspaceId { get; set; }
    public int CustomerId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}