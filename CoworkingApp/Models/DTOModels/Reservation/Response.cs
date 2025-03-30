namespace CoworkingApp.Models.DTOModels.Reservation;

// Response DTOs

public class ReservationsResponseDto : PaginationResponseDto
{
    public required IEnumerable<ReservationDto> Reservations { get; set; }
}