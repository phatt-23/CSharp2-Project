using System.Data;
using AutoFilterer.Attributes;
using AutoFilterer.Extensions;
using AutoFilterer.Types;
using AutoFilterer.Enums;
using CoworkingApp.Data;
using CoworkingApp.Models.DataModels;
using Microsoft.EntityFrameworkCore;

namespace CoworkingApp.Services.Repositories;

public interface IReservationRepository
{
    Task<Reservation> AddReservation(Reservation res);
    Task<IEnumerable<Reservation>> GetReservations(ReservationsFilter filter);
    Task<Reservation> UpdateReservation(Reservation res);
}

public class ReservationRepository
    (
        CoworkingDbContext context
    ) 
    : IReservationRepository
{
    public Task<IEnumerable<Reservation>> GetReservations(ReservationsFilter filter)
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

    public async Task<Reservation> AddReservation(Reservation res)
    {
        if (res.StartTime < DateTime.Now || res.EndTime < DateTime.Now)
            throw new ReservationTimeInPastException("START and END time cannot denote time in the past.")
                { PropertyName = "StartTime" };

        if (res.StartTime > res.EndTime)
            throw new ReservationTimeInPastException("START time must be before the END time")
                { PropertyName = "EndTime" };

        var workspace = await context.Workspaces
            .Where(w => w.WorkspaceId == res.WorkspaceId)
            .Include(w => w.WorkspacePricings)
            .FirstOrDefaultAsync();

        if (workspace == null)
            throw new WorkspaceNotFoundException($"Workspace with id {res.WorkspaceId} not found");
        
        // find the current pricing (time of reservation in range of the pricing)
        if (!workspace.WorkspacePricings.Any())
            throw new NoPricingForWorkspaceException("There is no pricing for this workspace");
        var latestValidFrom = workspace.WorkspacePricings.Max(p => p.ValidFrom);
            
        var workspacePricing = workspace.WorkspacePricings
            .Single(p => p.ValidFrom == latestValidFrom);
       
        if (workspacePricing == null)
            throw new ConstraintException(
                $"Workspace with id {res.WorkspaceId} doesn't have currently have a valid pricing.");

        var totalPrice = workspacePricing.PricePerHour * (decimal)(res.EndTime - res.StartTime).TotalHours;

        res.PricingId = workspacePricing.WorkspacePricingId;
        res.TotalPrice = totalPrice;
       
        var addedRes = await context.Reservations.AddAsync(res);
        await context.SaveChangesAsync();

        return addedRes.Entity;
    }

    public async Task<Reservation> UpdateReservation(Reservation res)
    {
        var updatedRes = context.Reservations.Update(res);
        await context.SaveChangesAsync();
        return updatedRes.Entity;
    }
}

public class ReservationsFilter : FilterBase
{
    [CompareTo(nameof(Reservation.ReservationId))] public int? Id { get; set; }
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


public class ReservationTimeInPastException(string m) : Exception(m)
{
    public string PropertyName { get; init; } = string.Empty;
}

public class WorkspaceNotFoundException(string m) : Exception(m);

public class NoPricingForWorkspaceException(string m) : Exception(m);