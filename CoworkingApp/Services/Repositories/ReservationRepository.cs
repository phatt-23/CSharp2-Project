using AutoFilterer.Attributes;
using AutoFilterer.Extensions;
using AutoFilterer.Types;
using AutoFilterer.Enums;
using CoworkingApp.Data;
using CoworkingApp.Models.DataModels;
using Microsoft.EntityFrameworkCore;
using System.Data;
using CoworkingApp.Models.Exceptions;

namespace CoworkingApp.Services.Repositories;

public interface IReservationRepository
{
    Task<Reservation> AddReservation(Reservation reservation);
    Task<IEnumerable<Reservation>> GetReservations(ReservationsFilter filter);
    Task<Reservation> UpdateReservation(Reservation reservation);
    Task<Reservation> CancelReservation(Reservation reservation);
}

public class ReservationRepository
    (
        CoworkingDbContext context
    ) 
    : IReservationRepository
{
    public async Task<IEnumerable<Reservation>> GetReservations(ReservationsFilter filter)
    {
        var query = context.Reservations.ApplyFilter(filter);
        
        query = filter.TotalPrice.ApplyTo(query, x => x.TotalPrice);
        query = filter.CreatedAt.ApplyTo(query, x => x.CreatedAt);

        if (filter.IncludeCustomer)
        {
            query = query.Include(r => r.Customer);
        }

        if (filter.IncludeWorkspacePricing)
        {
            query = query.Include(r => r.Pricing);
        }

        if (filter.IncludeWorkspace)
        {
            query = query.Include(r => r.Workspace);
        }

        query = (filter.Sort) switch
        {
            ReservationSort.PriceAsc => query.OrderBy(x => x.TotalPrice),
            ReservationSort.PriceDesc => query.OrderByDescending(x => x.TotalPrice),
            ReservationSort.StartTimeAsc => query.OrderBy(x => x.StartTime),
            ReservationSort.StartTimeDesc => query.OrderByDescending(x => x.StartTime),
            ReservationSort.None => query,
            _ => throw new NotImplementedException(),
        };

        return query;
    }

    public async Task<Reservation> AddReservation(Reservation reservation)
    {
        if (reservation.StartTime <= DateTime.Now || reservation.EndTime <= DateTime.Now)
        {
            throw new ReservationTimeInPastException("Start and end time must be future time.");
        }

        if (reservation.StartTime > reservation.EndTime)
        {
            throw new ReservationTimeInPastException("Start time must be before the end time");
        }

        var clashingReservationCount = await context.Reservations
            .Where(x => !x.IsCancelled && 
                x.WorkspaceId == reservation.WorkspaceId && 
                reservation.StartTime < x.EndTime && 
                reservation.EndTime > x.StartTime)
            .CountAsync();

        if (clashingReservationCount != 0)
        {
            throw new ClashingReservationTimeException("Reservation couldn't be finished because of clashing times with other reservations.");
        }

        var workspace = await context.Workspaces
            .Where(w => w.WorkspaceId == reservation.WorkspaceId)
            .Include(w => w.WorkspacePricings)
            .FirstOrDefaultAsync()
            ?? throw new WorkspaceNotFoundException($"Workspace with id {reservation.WorkspaceId} not found");


        // find the current pricing (time of reservation in range of the pricing)
        // if there aren't any throw an error
        if (workspace.WorkspacePricings.Count == 0)
        {
            throw new NoPricingForWorkspaceException("There is no pricing for this workspace");
        }

        var workspacePricing = workspace.GetCurrentPricing()
            ?? throw new ConstraintException($"Workspace with id {reservation.WorkspaceId} doesn't have currently have a valid pricing.");

        var totalPrice = workspacePricing.PricePerHour * (decimal)(reservation.EndTime - reservation.StartTime).TotalHours;

        reservation.PricingId = workspacePricing.WorkspacePricingId;
        reservation.TotalPrice = totalPrice;
       
        var addedReservation = await context.Reservations.AddAsync(reservation);

        await context.SaveChangesAsync();
        return addedReservation.Entity;
    }

    public async Task<Reservation> CancelReservation(Reservation reservation)
    {
        reservation.IsCancelled = true;
        var cancelledReservation = context.Reservations.Update(reservation);
        await context.SaveChangesAsync();
        return cancelledReservation.Entity;
    }

    public async Task<Reservation> UpdateReservation(Reservation newReservation)
    {
        var oldReservation = await context.Reservations
            .Include(x => x.Workspace)
            .FirstAsync(x => x.ReservationId == newReservation.ReservationId)
            ?? throw new NotFoundException($"Reservation with id {newReservation.ReservationId} not found");


        // check if reservation with id exists
        if (await context.Reservations.FindAsync(newReservation.ReservationId) == null)
        {
            throw new Exception($"Reservation with id {newReservation.ReservationId} not found");
        }

        // do the same checks as for adding a reservation
        if (newReservation.StartTime <= DateTime.Now || newReservation.EndTime <= DateTime.Now)
        {
            throw new ReservationTimeInPastException("Start and end time must be future time.");
        }

        if (newReservation.StartTime > newReservation.EndTime)
        {
            throw new ReservationTimeInPastException("Start time must be before the end time");
        }

        var clashingReservations = await context.Reservations
            .Where(x => !x.IsCancelled &&
                x.ReservationId != oldReservation.ReservationId &&
                x.WorkspaceId == oldReservation.WorkspaceId &&
                newReservation.StartTime < x.EndTime &&
                newReservation.EndTime > x.StartTime)
            .ToListAsync();

        if (clashingReservations.Count != 0)
        {
            throw new ClashingReservationTimeException("Reservation couldn't be finished because of clashing times with other reservations.");
        }

        var workspace = await context.Workspaces
            .Where(w => w.WorkspaceId == oldReservation.WorkspaceId)
            .Include(w => w.WorkspacePricings)
            .SingleOrDefaultAsync()
            ?? throw new WorkspaceNotFoundException($"Workspace with id {newReservation.WorkspaceId} not found");

        // find the current pricing (time of reservation in range of the pricing)
        // if there aren't any throw an error
        if (workspace.WorkspacePricings.Count == 0)
        {
            throw new NoPricingForWorkspaceException("There is no pricing for this workspace");
        }

        var workspacePricing = workspace.GetCurrentPricing()
            ?? throw new ConstraintException($"Workspace with id {newReservation.WorkspaceId} doesn't have currently have a valid pricing.");

        var totalPrice = workspacePricing.PricePerHour * (decimal)(newReservation.EndTime - newReservation.StartTime).TotalHours;

        oldReservation.PricingId = workspacePricing.WorkspacePricingId;
        oldReservation.TotalPrice = totalPrice;
        oldReservation.StartTime = newReservation.StartTime;
        oldReservation.EndTime = newReservation.EndTime;

        var updatedRes = context.Reservations.Update(oldReservation);
        await context.SaveChangesAsync();
        return updatedRes.Entity;
    }
}

public enum ReservationSort
{
    None,
    PriceDesc,
    PriceAsc,
    StartTimeDesc,
    StartTimeAsc,
}

public class ReservationsFilter : FilterBase
{
    [CompareTo(nameof(Reservation.ReservationId))] 
    public int? Id { get; set; }

    [CompareTo(nameof(Reservation.WorkspaceId))] 
    public int? WorkspaceId { get; set; }
    
    [CompareTo(nameof(Reservation.StartTime))]
    [OperatorComparison(OperatorType.GreaterThanOrEqual)]
    public DateTime? StartTime { get; set; }

    [CompareTo(nameof(Reservation.StartTime))]
    [OperatorComparison(OperatorType.LessThanOrEqual)]
    public DateTime? EndTime { get; set; }

    public NullableRangeFilter<decimal> TotalPrice { get; set; } = new();

    [CompareTo(nameof(Reservation.PricingId))] 
    public int? PricingId { get; set; }

    public RangeFilter<DateTime> CreatedAt { get; set; } = new();

    [CompareTo(nameof(Reservation.CustomerId))] 
    public int? CustomerId { get; set; }

    [CompareTo(nameof(Reservation.IsCancelled))] 
    public bool? IsCancelled { get; set; }

    public bool IncludeCustomer { get; set; } = false;
    public bool IncludeWorkspacePricing { get; set; } = false;
    public bool IncludeWorkspace { get; set; } = false;

    public ReservationSort Sort { get; set; } = ReservationSort.None;
}


