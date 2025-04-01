using System.Data;
using AutoFilterer.Attributes;
using AutoFilterer.Extensions;
using AutoFilterer.Types;
using AutoFilterer.Enums;
using CoworkingApp.Data;
using CoworkingApp.Models.DataModels;
using CoworkingApp.Models.DTOModels.Reservation;
using CoworkingApp.Models.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace CoworkingApp.Services;

public interface IReservationRepository
{
    Task<IEnumerable<Reservation>> GetReservationsAsync(ReservationsFilter filter);
    Task<Reservation> AddReservationAsync(Reservation res);
    Task<Reservation> UpdateReservationAsync(Reservation res);
}


public class ReservationRepository(CoworkingDbContext context) : IReservationRepository
{
    public Task<IEnumerable<Reservation>> GetReservationsAsync(ReservationsFilter filter)
    {
        var query = context.Reservations.ApplyFilter(filter);
        
        query = filter.TotalPrice.ApplyTo(query, x => x.TotalPrice);
        query = filter.CreatedAt.ApplyTo(query, x => x.CreatedAt);

        if (filter.IncludeCustomer)
            query = query.Include(r => r.Customer);

        if (filter.IncludeWorkspacePricing)
            query = query.Include(r => r.Pricing);

        if (filter.IncludeWorkspace)
            query = query.Include(r => r.Workspace);

        return Task.FromResult<IEnumerable<Reservation>>(query);
    }

    public async Task<Reservation> AddReservationAsync(Reservation res)
    {
        if (res.StartTime < DateTime.Now || res.EndTime < DateTime.Now)
            throw new InvalidOperationException("START and END time cannot denote time in the past."); 
        
        if (res.StartTime > res.EndTime)
            throw new InvalidOperationException("START time must be before the END time");

        var workspace = await context.Workspaces
            .Where(w => w.Id == res.WorkspaceId)
            .Include(w => w.Status)
            .Include(w => w.WorkspacePricings)
            .FirstOrDefaultAsync();

        if (workspace == null)
            throw new NotFoundException($"Workspace with id {res.WorkspaceId} not found");
        
        if (workspace.Status.Type != WorkspaceStatusType.Available)
            throw new InvalidOperationException("Workspace is not available");
       
        // find the current pricing (time of reservation in range of the pricing)
        var latestValidFrom = workspace.WorkspacePricings.Max(p => p.ValidFrom);
            
        var workspacePricing = workspace.WorkspacePricings
            .Single(p => p.ValidFrom == latestValidFrom);
       
        if (workspacePricing == null)
            throw new ConstraintException(
                $"Workspace with id {res.WorkspaceId} doesn't have currently have a valid pricing.");

        var totalPrice = workspacePricing.PricePerHour * (decimal)(res.EndTime - res.StartTime).TotalHours;

        res.PricingId = workspacePricing.Id;
        res.TotalPrice = totalPrice;
       
        var addedRes = await context.Reservations.AddAsync(res);
        await context.SaveChangesAsync();

        return addedRes.Entity;
    }

    public async Task<Reservation> UpdateReservationAsync(Reservation res)
    {
        var updatedRes = context.Reservations.Update(res);
        await context.SaveChangesAsync();
        return updatedRes.Entity;
    }
}

public class ReservationsFilter : FilterBase
{
    [CompareTo(nameof(Reservation.Id))] public int? Id { get; set; }
    [CompareTo(nameof(Reservation.WorkspaceId))] public int? WorkspaceId { get; set; }
    
    [CompareTo(nameof(Reservation.StartTime))]
    [OperatorComparison(OperatorType.GreaterThanOrEqual)]
    public DateTime? StartTime { get; set; }

    [CompareTo(nameof(Reservation.StartTime))]
    [OperatorComparison(OperatorType.LessThanOrEqual)]
    public DateTime? EndTime { get; set; }

    public NullableRangeFilter<decimal> TotalPrice { get; set; } = new();
    [CompareTo(nameof(Reservation.PricingId))] public int? PricingId { get; set; }
    public RangeFilter<DateTime> CreatedAt { get; set; } = new();
    [CompareTo(nameof(Reservation.CustomerId))] public int? CustomerId { get; set; }
    [CompareTo(nameof(Reservation.IsCancelled))] public bool? IsCancelled { get; set; }
    public bool IncludeCustomer { get; set; } = false;
    public bool IncludeWorkspacePricing { get; set; } = false;
    public bool IncludeWorkspace { get; set; } = false;
}
