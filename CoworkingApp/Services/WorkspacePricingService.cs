using AutoMapper;
using CoworkingApp.Models.DataModels;
using CoworkingApp.Models.Exceptions;
using CoworkingApp.Services;
using CoworkingApp.Services.Repositories;
using Microsoft.EntityFrameworkCore;


public interface IWorkspacePricingService
{
    Task<IEnumerable<WorkspacePricing>> GetPricingsAsync(WorkspacePricingQueryRequestDto request);
    Task<WorkspacePricing> CreateWorkspacePricingAsync(WorkspacePricingCreateRequestDto request);
    Task<WorkspacePricing> GetLatestWorkspacePricingOfWorkspaceAsync(Workspace workspace);
}


public class WorkspacePricingService
    (
    IWorkspacePricingRepository pricingRepository,
    IWorkspaceRepository workspaceRepository,
    IMapper mapper
    ) 
    : IWorkspacePricingService
{
    public async Task<IEnumerable<WorkspacePricing>> GetPricingsAsync(WorkspacePricingQueryRequestDto request)
    {
        var pricings = await pricingRepository.GetWorkspacePricingsAsync(new WorkspacePricingFilter 
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

    public async Task<WorkspacePricing> CreateWorkspacePricingAsync(WorkspacePricingCreateRequestDto request)
    {
        // check date timing
        var now = DateTime.Now;

        if (request is { ValidFrom: not null, ValidUntil: not null })
        {
            if (request.ValidFrom <= now || request.ValidUntil <= now)
                throw new Exception("Dates must be in the future");
            
            if (request.ValidFrom > request.ValidUntil)
                throw new Exception("Valid from must be before valid until");
        }
        else
        {
            request.ValidFrom = now;
            request.ValidUntil = null;  // just to be more obvious
        }
        
        // check if workspace exists (throws)
        var workspaces = await workspaceRepository.GetWorkspacesAsync(new WorkspaceFilter { Id = request.WorkspaceId });
        var workspace = workspaces.Single();
        
        // update the latest pricing
        var latestPricing = await GetLatestWorkspacePricingOfWorkspaceAsync(workspace);

        latestPricing.ValidUntil = request.ValidFrom;

        await pricingRepository.UpdateWorkspacePricingAsync(latestPricing);

        var newPricing = mapper.Map<WorkspacePricing>(request);

        return await pricingRepository.AddWorkspacePricingAsync(newPricing);
    }


    public async Task<WorkspacePricing> GetLatestWorkspacePricingOfWorkspaceAsync(Workspace workspace)
    {
        var pricings = await pricingRepository.GetWorkspacePricingsAsync(
            new WorkspacePricingFilter { WorkspaceId = workspace.WorkspaceId });
        
        var query = pricings.AsQueryable();

        if (!query.Any())
            throw new Exception("There are no workspace pricing records.");
        
        var latestValidFrom = await query.MaxAsync(x => x.ValidFrom);

        var latestPricing = await query
                                .Where(x => x.ValidFrom == latestValidFrom)
                                .SingleOrDefaultAsync() 
                            ?? throw new NotFoundException($"Workspace with id '{workspace.WorkspaceId}' doesn't have a latest workspace pricing.");
    
        return latestPricing;
    }
}

