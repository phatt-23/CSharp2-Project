using AutoMapper;
using CoworkingApp.Models.DataModels;
using CoworkingApp.Models.DtoModels;
using CoworkingApp.Models.Exceptions;
using CoworkingApp.Services.Repositories;

namespace CoworkingApp.Services;

public interface IReservationService
{
    Task<IEnumerable<Reservation>> GetReservations(ReservationQueryRequestDto request);
    Task<IEnumerable<Reservation>> GetReservationsForAdmin(AdminReservationQueryRequestDto request);
    Task<Reservation> GetReservationById(int reservationId);
    Task<Reservation> CreateReservation(int customerId, ReservationCreateRequestDto request);
    Task<Reservation> UpdateReservation(int reservationId, ReservationUpdateRequestDto request);
    Task<Reservation> CancelReservation(int reservationId);
}


public class ReservationService
    (
        IReservationRepository repository,
        IMapper mapper
    ) 
    : IReservationService
{
    public async Task<IEnumerable<Reservation>> GetReservations(ReservationQueryRequestDto request)
    {
        var reservations = await repository.GetReservations(new ReservationsFilter
        {
            WorkspaceId = request.WorkspaceId,
            CustomerId = request.CustomerId,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            TotalPrice = request.Price,
            PricingId = request.PricingId,
        });

        return reservations;
    }

    public async Task<IEnumerable<Reservation>> GetReservationsForAdmin(AdminReservationQueryRequestDto request)
    {
        return await GetReservations(request);
    }

    public async Task<Reservation> GetReservationById(int reservationId)
    {
        var reservations = await repository.GetReservations(new ReservationsFilter { Id = reservationId });

        if (!reservations.Any()) 
            throw new NotFoundException($"Reservation with id '{reservationId}' not found");

        return reservations.First();
    }

    public async Task<Reservation> CreateReservation(int customerId, ReservationCreateRequestDto request)
    {
        var reservation = mapper.Map<Reservation>(request);
        reservation.CustomerId = customerId;
        return await repository.AddReservation(reservation);
    }

    public async Task<Reservation> UpdateReservation(int reservationId, ReservationUpdateRequestDto request)
    {
        var reservation = await GetReservationById(reservationId);

        if (request.WorkspaceId != null)    reservation.WorkspaceId = request.WorkspaceId.Value;
        if (request.StartTime != null)      reservation.StartTime = request.StartTime.Value;
        if (request.EndTime != null)        reservation.EndTime = request.EndTime.Value;
        if (request.IsCancelled != null)    reservation.IsCancelled = request.IsCancelled.Value;

        return await repository.UpdateReservation(reservation);
    }

    public async Task<Reservation> CancelReservation(int reservationId)
    {
        var reservations = await repository.GetReservations(new ReservationsFilter { Id = reservationId });
        
        var reservation = reservations.FirstOrDefault();

        if (reservation == null)
            throw new NotFoundException($"Reservation with id {reservationId} not found");

        if (reservation.StartTime <= DateTime.Now)
            throw new ReservationAlreadyTakingPlaceException("Cannot cancel reservation that is already taking place");

        reservation.IsCancelled = true;
        return await repository.UpdateReservation(reservation);
    }
}


public class ReservationAlreadyTakingPlaceException(string m) : Exception(m);