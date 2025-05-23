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
    Task<Reservation> UpdateReservation(int customerId, ReservationUpdateRequestDto request);
    Task<Reservation> CancelReservation(int reservationId);

    // Admin
    Task<IEnumerable<Reservation>> AdminGetReservations(AdminReservationQueryRequestDto request);
    Task<Reservation> AdminUpdateReservation(AdminReservationUpdateRequestDto request);
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
            IncludeCustomer = true,
            IncludeWorkspace = true,
            IncludeWorkspacePricing = true,
            Sort = ReservationSort.StartTimeDesc
        });

        return reservations;
    }

    public async Task<IEnumerable<Reservation>> AdminGetReservations(AdminReservationQueryRequestDto request)
    {
        var res = await GetReservations(request);

        if (request.CustomerId != null)
            res = res.Where(r => r.CustomerId == request.CustomerId);

        return res;
    }

    public async Task<Reservation> GetReservationById(int reservationId)
    {
        var reservations = await repository.GetReservations(new ReservationsFilter 
        { 
            Id = reservationId,
            IncludeWorkspace = true,
            IncludeWorkspacePricing = true,
            IncludeCustomer = true,
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
        var res = await repository.AddReservation(reservation);
        return await GetReservationById(res.ReservationId);
    }

    public async Task<Reservation> UpdateReservation(int customerId, ReservationUpdateRequestDto request)
    {
        var reservation = mapper.Map<Reservation>(request);
        reservation.CustomerId = customerId;
        var updated = await repository.UpdateReservation(reservation);
        return await GetReservationById(updated.ReservationId);
    }

    public async Task<Reservation> AdminUpdateReservation(AdminReservationUpdateRequestDto request)
    {
        var res = mapper.Map<Reservation>(request);
        var updated = await repository.UpdateReservation(res);
        return await GetReservationById(updated.ReservationId);
    }

    public async Task<Reservation> CancelReservation(int reservationId)
    {
        var reservation = await GetReservationById(reservationId);

        if (reservation.StartTime <= DateTime.Now)
        {
            throw new ReservationAlreadyTakingPlaceException("Cannot cancel reservation that is already taking place");
        }

        var res = await repository.CancelReservation(reservation);
        return await GetReservationById(res.ReservationId);
    }
}

