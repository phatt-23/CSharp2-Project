using AutoMapper;
using CoworkingApp.Models.DataModels;
using CoworkingApp.Models.DtoModels;
using CoworkingApp.Models.Exceptions;
using CoworkingApp.Services.Repositories;

namespace CoworkingApp.Services;

public interface IReservationService
{
    // Public
    Task<IEnumerable<Reservation>> GetReservations(ReservationQueryRequestDto request);
    Task<Reservation> GetReservationById(int reservationId);
    Task<Reservation> CreateReservation(int customerId, ReservationCreateRequestDto request);
    Task<Reservation> UpdateReservation(ReservationUpdateRequestDto request);
    Task<Reservation> CancelReservation(int reservationId);

    // Admin
    Task<IEnumerable<Reservation>> AdminGetReservations(AdminReservationQueryRequestDto request);
    Task<Reservation> AdminUpdateReservation(int reservationId, AdminReservationUpdateRequestDto request);
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
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            TotalPrice = request.Price,
        });

        return reservations;
    }

    public async Task<IEnumerable<Reservation>> AdminGetReservations(AdminReservationQueryRequestDto request)
    {
        return await GetReservations(request);
    }

    public async Task<Reservation> GetReservationById(int reservationId)
    {
        var reservations = await repository.GetReservations(new ReservationsFilter 
        { 
            Id = reservationId,
            IncludeWorkspace = true,
            IncludeWorkspacePricing = true,
        });

        if (!reservations.Any())
        { 
            throw new NotFoundException($"Reservation with id '{reservationId}' not found");
        }

        return reservations.Single();
    }

    public async Task<Reservation> CreateReservation(int customerId, ReservationCreateRequestDto request)
    {
        var reservation = mapper.Map<Reservation>(request);
        reservation.CustomerId = customerId;
        return await repository.AddReservation(reservation);
    }

    public async Task<Reservation> UpdateReservation(ReservationUpdateRequestDto request)
    {
        return await repository.UpdateReservation(mapper.Map<Reservation>(request));
    }

    public async Task<Reservation> AdminUpdateReservation(int reservationId, AdminReservationUpdateRequestDto request)
    {
        var reservation = await GetReservationById(reservationId);
        return await repository.UpdateReservation(mapper.Map<Reservation>(request));
    }

    public async Task<Reservation> CancelReservation(int reservationId)
    {
        var reservation = await GetReservationById(reservationId);

        if (reservation.StartTime <= DateTime.Now)
        {
            throw new ReservationAlreadyTakingPlaceException("Cannot cancel reservation that is already taking place");
        }

        return await repository.CancelReservation(reservation);
    }
}

