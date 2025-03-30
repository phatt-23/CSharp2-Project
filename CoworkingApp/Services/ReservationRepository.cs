using System.Data;
using CoworkingApp.Data;
using CoworkingApp.Models.DataModels;
using CoworkingApp.Models.DTOModels.Reservation;
using CoworkingApp.Models.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace CoworkingApp.Services;

public interface IReservationRepository
{
    Task<IEnumerable<Reservation>> GetReservationsAsync(ReservationsFilterOptions options);
    Task<Reservation> AddReservationAsync(Reservation res);
    Task<Reservation> UpdateReservationAsync(Reservation res);
}


public class ReservationRepository(CoworkingDbContext context) : IReservationRepository
{
    public Task<IEnumerable<Reservation>> GetReservationsAsync(ReservationsFilterOptions options)
    {
        var rs = context.Reservations
            .Where(r => options.Id == null || options.Id == r.Id)
            .Where(r => options.WorkspaceId == null || options.WorkspaceId == r.WorkspaceId)
            .Where(r => options.StartTime == null || options.StartTime <= r.StartTime)
            .Where(r => options.EndTime == null || options.EndTime >= r.EndTime)
            .Where(r => options.TotalPriceLow == null || options.TotalPriceLow <= r.TotalPrice)
            .Where(r => options.TotalPriceHigh == null || options.TotalPriceHigh >= r.TotalPrice)
            .Where(r => options.PricingId == null || options.PricingId == r.PricingId)
            .Where(r => options.CreatedAtLow == null || options.CreatedAtLow <= r.CreatedAt)
            .Where(r => options.CreatedAtHigh == null || options.CreatedAtHigh >= r.CreatedAt)
            .Where(r => options.CustomerId == null || options.CustomerId == r.CustomerId)
            .Where(r => options.IsCancelled == null || options.IsCancelled == r.IsCancelled);

        if (options.IncludeCustomer)
            rs = rs.Include(r => r.Customer);

        if (options.IncludeWorkspacePricing)
            rs = rs.Include(r => r.Pricing);

        if (options.IncludeWorkspace)
            rs = rs.Include(r => r.Workspace);

        return Task.FromResult<IEnumerable<Reservation>>(rs);
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

public class ReservationsFilterOptions
{
    public int? Id { get; set; }
    public int? WorkspaceId { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public decimal? TotalPriceLow { get; set; }
    public decimal? TotalPriceHigh { get; set; }
    public int? PricingId { get; set; }
    public DateTime? CreatedAtLow { get; set; }
    public DateTime? CreatedAtHigh { get; set; }
    public int? CustomerId { get; set; }
    public bool? IsCancelled { get; set; }
    public bool IncludeCustomer { get; set; } = false;
    public bool IncludeWorkspacePricing { get; set; } = false;
    public bool IncludeWorkspace { get; set; } = false;
}
