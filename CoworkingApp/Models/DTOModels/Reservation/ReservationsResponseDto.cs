namespace CoworkingApp.Models.DTOModels.Reservation;

public class ReservationsResponseDto : PaginationResponseDto
{
    public required IEnumerable<ReservationDto> Reservation { get; set; }
}