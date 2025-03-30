using System.Data;
using System.Globalization;
using AutoMapper;
using CoworkingApp.Data;
using CoworkingApp.Models.DataModels;
using CoworkingApp.Models.DTOModels.Reservation;
using CoworkingApp.Models.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace CoworkingApp.Services;


public interface IReservationsService
{
    Task<IEnumerable<Reservation>> GetReservationsAsync(ReservationQueryRequestDto request);
    Task<Reservation> GetReservationByIdAsync(int reservationId);
    Task<Reservation> CreateReservationAsync(ReservationCreateRequestDto request);
    Task<Reservation> CancelReservationAsync(int id);
}


public class ReservationsService(
    IReservationRepository reservationRepository,
    IMapper mapper
    ) : IReservationsService
{
    public async Task<IEnumerable<Reservation>> GetReservationsAsync(ReservationQueryRequestDto request)
    {
        var rs = await reservationRepository.GetReservationsAsync(new ReservationsFilterOptions
        {
            WorkspaceId = request.WorkspaceId,
            CustomerId = request.CustomerId,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            TotalPriceLow = request.PriceLow,
            TotalPriceHigh = request.PriceHigh,
            PricingId = request.PricingId,
        });

        return rs;
    }

    public async Task<Reservation> GetReservationByIdAsync(int reservationId)
    {
        var rs = await reservationRepository.GetReservationsAsync(new ReservationsFilterOptions { Id = reservationId });
        if (!rs.Any()) throw new NotFoundException($"Reservation with id '{reservationId}' not found");
        return rs.First();
    }

    public async Task<Reservation> CreateReservationAsync(ReservationCreateRequestDto request)
    {
        var res = await reservationRepository.AddReservationAsync(mapper.Map<Reservation>(request));
        return res;
    }

    public async Task<Reservation> CancelReservationAsync(int reservationId)
    {
        var rs = await reservationRepository.GetReservationsAsync(
            new ReservationsFilterOptions { Id = reservationId });
        
        var res = rs.FirstOrDefault();
        if (res == null) 
            throw new NotFoundException($"Reservation with id {reservationId} not found");

        if (res.StartTime >= DateTime.Now)
            throw new InvalidOperationException("Cannot cancel reservation that is already taking place");
       
        res.IsCancelled = true;
        return await reservationRepository.UpdateReservationAsync(res);
    }
}

