using System.Data;
using System.Globalization;
using AutoMapper;
using CoworkingApp.Data;
using CoworkingApp.Models.DataModels;
using CoworkingApp.Models.DTOModels.Reservation;
using CoworkingApp.Models.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace CoworkingApp.Services;


public interface IReservationService
{
    Task<IEnumerable<Reservation>> GetReservationsAsync(ReservationQueryRequestDto request);
    Task<IEnumerable<Reservation>> GetReservationsForAdminAsync(AdminReservationQueryRequestDto request);
    Task<Reservation> GetReservationByIdAsync(int reservationId);
    Task<Reservation> CreateReservationAsync(int customerId, ReservationCreateRequestDto request);
    Task<Reservation> UpdateReservationAsync(int reservationId, ReservationUpdateRequestDto request);
    Task<Reservation> CancelReservationAsync(int id);
}


public class ReservationService(
    IReservationRepository reservationRepository,
    IMapper mapper
    ) : IReservationService
{
    public async Task<IEnumerable<Reservation>> GetReservationsAsync(ReservationQueryRequestDto request)
    {
        var rs = await reservationRepository.GetReservationsAsync(new ReservationsFilter
        {
            WorkspaceId = request.WorkspaceId,
            CustomerId = request.CustomerId,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            TotalPrice = request.Price,
            PricingId = request.PricingId,
        });

        return rs;
    }

    public async Task<IEnumerable<Reservation>> GetReservationsForAdminAsync(AdminReservationQueryRequestDto request)
    {
        return await GetReservationsAsync(request);
    }


    public async Task<Reservation> GetReservationByIdAsync(int reservationId)
    {
        var rs = await reservationRepository.GetReservationsAsync(new ReservationsFilter { Id = reservationId });
        if (!rs.Any()) throw new NotFoundException($"Reservation with id '{reservationId}' not found");
        return rs.First();
    }

    public async Task<Reservation> CreateReservationAsync(int customerId, ReservationCreateRequestDto request)
    {
        var newReservation = mapper.Map<Reservation>(request);
        newReservation.CustomerId = customerId;
        var res = await reservationRepository.AddReservationAsync(newReservation);
        return res;
    }

    public async Task<Reservation> UpdateReservationAsync(int reservationId, ReservationUpdateRequestDto request)
    {
        var reservation = await GetReservationByIdAsync(reservationId);
        if (request.WorkspaceId != null) reservation.WorkspaceId = request.WorkspaceId.Value;
        if (request.StartTime != null) reservation.StartTime = request.StartTime.Value;
        if (request.EndTime != null) reservation.EndTime = request.EndTime.Value;
        if (request.IsCancelled != null) reservation.IsCancelled = request.IsCancelled.Value;
        return await reservationRepository.UpdateReservationAsync(reservation);
    }


    public async Task<Reservation> CancelReservationAsync(int reservationId)
    {
        var rs = await reservationRepository.GetReservationsAsync(
            new ReservationsFilter { Id = reservationId });
        
        var res = rs.FirstOrDefault();
        if (res == null) 
            throw new NotFoundException($"Reservation with id {reservationId} not found");

        if (res.StartTime <= DateTime.Now)
            throw new InvalidOperationException("Cannot cancel reservation that is already taking place");
       
        res.IsCancelled = true;
        return await reservationRepository.UpdateReservationAsync(res);
    }
}

