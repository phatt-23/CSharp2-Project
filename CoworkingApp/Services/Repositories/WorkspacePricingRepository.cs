using AutoFilterer.Attributes;
using AutoFilterer.Enums;
using AutoFilterer.Extensions;
using AutoFilterer.Types;
using CoworkingApp.Data;
using CoworkingApp.Models.DataModels;
using CoworkingApp.Models.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace CoworkingApp.Services.Repositories;

public interface IWorkspacePricingRepository
{
    Task<IEnumerable<WorkspacePricing>> GetPricings(WorkspacePricingFilter filter);
    Task<WorkspacePricing> AddPricing(WorkspacePricing pricing);
    Task<WorkspacePricing> UpdatePricing(WorkspacePricing pricing);
}

public class WorkspacePricingRepository
    (
        CoworkingDbContext context
    ) 
    : IWorkspacePricingRepository
{
    public async Task<IEnumerable<WorkspacePricing>> GetPricings(WorkspacePricingFilter filter)
    {
        var query = context.WorkspacePricings.ApplyFilter(filter);

        query = filter.PricePerHour.ApplyTo(query, x => x.PricePerHour);

        if (filter.IncludeReservations)
            query = query.Include(x => x.Reservations);

        if (filter.IncludeWorkspace)
            query = query.Include(x => x.Workspace);

        return query;
    }

    public async Task<WorkspacePricing> AddPricing(WorkspacePricing pricing)
    {
        var addedPricing = await context.WorkspacePricings.AddAsync(pricing);
        await context.SaveChangesAsync();
        return addedPricing.Entity;
    }

    public async Task<WorkspacePricing> UpdatePricing(WorkspacePricing pricing)
    {
        var updatedPricing = context.WorkspacePricings.Update(pricing);
        await context.SaveChangesAsync();
        return updatedPricing.Entity;
    }
}

public class WorkspacePricingFilter : FilterBase
{
    [CompareTo(nameof(WorkspacePricing.WorkspacePricingId))] 
    public int? Id { get; set; }
    
    [CompareTo(nameof(WorkspacePricing.WorkspaceId))] 
    public int? WorkspaceId { get; set; }

    // [CompareTo(nameof(WorkspacePricing.PricePerHour))]
    public RangeFilter<decimal> PricePerHour { get; set; } = new();

    [CompareTo(nameof(WorkspacePricing.ValidFrom))]
    [OperatorComparison(OperatorType.GreaterThanOrEqual)]
    public DateTime? ValidFrom { get; set; }

    [CompareTo(nameof(WorkspacePricing.ValidUntil))]
    [OperatorComparison(OperatorType.LessThanOrEqual)]
    public DateTime? ValidUntil { get; set; }

    public bool IncludeReservations { get; set; } = false;
    public bool IncludeWorkspace { get; set; } = false;
}

