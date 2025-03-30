using System.Data;
using System.Globalization;
using AutoMapper;
using CoworkingApp.Data;
using CoworkingApp.Models.DataModels;
using CoworkingApp.Models.DTOModels.Reservation;
using CoworkingApp.Models.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace CoworkingApp.Services;

public class ReservationsService(
    CoworkingDbContext context,
    IMapper mapper
    ) 
{
    public async Task<(IEnumerable<ReservationDto>, int)> GetAsync(ReservationsQueryDto query)
    {
        var reservations = context.Reservations.AsQueryable();
        
        if (query.WorkspaceId != null)
            reservations = reservations.Where(r => r.WorkspaceId == query.WorkspaceId);
        if (query.StartTime != null)
            reservations = reservations.Where(r => r.StartTime >= query.StartTime);
        if (query.EndTime != null)
            reservations = reservations.Where(r => r.EndTime <= query.EndTime);
        if (query.TotalPrice != null)
            reservations = reservations.Where(r => r.TotalPrice >= query.TotalPrice);
        if (query.PricingId != null)
            reservations = reservations.Where(r => r.PricingId == query.PricingId);
        if (query.CustomerId != null)
            reservations = reservations.Where(r => r.CustomerId == query.CustomerId);
        if (query.PriceLow != null)
            reservations = reservations.Where(r => r.TotalPrice >= query.PriceLow);
        if (query.PriceHigh != null)
            reservations = reservations.Where(r => r.TotalPrice <= query.PriceHigh);
            
        var totalCount = await reservations.CountAsync();
        
        reservations = reservations
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize);
        
        var reservationDtos = mapper.Map<IEnumerable<ReservationDto>>(reservations);
        
        return (reservationDtos, totalCount);
    }

    public async Task<ReservationDto> GetByIdAsync(int id)
    {
        var reservation = await context.Reservations
            .Where(r => r.Id == id)
            .FirstOrDefaultAsync();
       
        if (reservation is null) 
            throw new NotFoundException($"Reservation with id '{id}' not found");

        return mapper.Map<ReservationDto>(reservation);
    }

    public async Task<ReservationDto> CreateAsync(ReservationCreateRequestDto request)
    {
        if (request.StartTime < DateTime.Now || request.EndTime < DateTime.Now)
            throw new InvalidOperationException("START and END time cannot denote time in the past."); 
        
        if (request.StartTime > request.EndTime)
            throw new InvalidOperationException("START time must be before the END time");

        var workspace = await context.Workspaces
            .Where(w => w.Id == request.WorkspaceId)
            .Include(w => w.Status)
            .Include(w => w.WorkspacePricings)
            .FirstOrDefaultAsync();

        if (workspace == null)
            throw new NotFoundException($"Workspace with id {request.WorkspaceId} not found");
        
        if (workspace.Status.Type != WorkspaceStatusType.Available)
            throw new InvalidOperationException("Workspace is not available");
       
        // find the current pricing (time of reservation in range of the pricing)
        var latestValidFrom = workspace.WorkspacePricings.Max(p => p.ValidFrom);
            
        var workspacePricing = workspace.WorkspacePricings
            .Single(p => p.ValidFrom == latestValidFrom);
       
        if (workspacePricing == null)
            throw new ConstraintException(
                $"Workspace with id {request.WorkspaceId} doesn't have currently have a valid pricing.");

        var totalPrice = workspacePricing.PricePerHour * (decimal)(request.EndTime - request.StartTime).TotalHours;
        

        var reservation = mapper.Map<Reservation>(request);
        reservation.PricingId = workspacePricing.Id;
        reservation.TotalPrice = totalPrice;
       
        context.Reservations.Add(reservation);
        await context.SaveChangesAsync();
        
        return mapper.Map<ReservationDto>(reservation);
    }

    public async Task<ReservationDto> CancelAsync(int id)
    {
        var reservation = await context.Reservations.FindAsync(id);
        if (reservation == null)
            throw new NotFoundException($"Reservation with id '{id}' not found");
        
        reservation.IsCancelled = true; 
        context.Reservations.Update(reservation); 
        await context.SaveChangesAsync();
        
        return mapper.Map<ReservationDto>(reservation);
    }
}