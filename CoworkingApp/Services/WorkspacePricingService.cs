using CoworkingApp.Models.DataModels;
using CoworkingApp.Models.Exceptions;
using CoworkingApp.Services;
using Microsoft.EntityFrameworkCore;


public interface IWorkspacePricingService
{
    Task<IEnumerable<WorkspacePricing>> GetPricingsAsync(PricingQueryRequestDto request);
    Task<WorkspacePricing> GetLatestWorkspacePricingOfWorkspaceAsync(Workspace workspace);
}


public class WorkspacePricingService(IWorkspacePricingRepository repository) : IWorkspacePricingService
{
    public async Task<IEnumerable<WorkspacePricing>> GetPricingsAsync(PricingQueryRequestDto request)
    {
        var pricings = await repository.GetWorkspacePricingsAsync(new WorkspacePricingFilter 
        {
            WorkspaceId = request.WorkspaceId,
            PricePerHour = request.PricePerHour,
            ValidFrom = request.ValidFrom,
            ValidUntil = request.ValidUntil,
            IncludeWorkspace = request.IncludeWorkspace,
            IncludeReservations = request.IncludeReservations,
        });

        return pricings;
    }

    public async Task<WorkspacePricing> GetLatestWorkspacePricingOfWorkspaceAsync(Workspace workspace)
    {
        var pricings = await repository.GetWorkspacePricingsAsync(
            new WorkspacePricingFilter { WorkspaceId = workspace.Id });
        
        var query = pricings.AsQueryable();

        if (!query.Any())
            throw new Exception("There are no workspace pricing records.");
        
        var latestValidFrom = await query.MaxAsync(x => x.ValidFrom);

        var latestPricing = await query
                                .Where(x => x.ValidFrom == latestValidFrom)
                                .SingleOrDefaultAsync() 
                            ?? throw new NotFoundException($"Workspace with id '{workspace.Id}' doesn't have a latest workspace pricing.");
    
        return latestPricing;
    }
}

