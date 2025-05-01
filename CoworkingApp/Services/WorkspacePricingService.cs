using AutoMapper;
using CoworkingApp.Data;
using CoworkingApp.Models.DataModels;
using CoworkingApp.Models.DtoModels;
using CoworkingApp.Models.Exceptions;
using CoworkingApp.Services.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CoworkingApp.Services;

public interface IWorkspacePricingService
{
    Task<IEnumerable<WorkspacePricing>> GetPricings(WorkspacePricingQueryRequestDto request);
    Task<WorkspacePricing> GetLatestPricingOfWorkspace(Workspace workspace);
    Task<WorkspacePricing> CreatePricing(WorkspacePricingCreateRequestDto request);
}


public class WorkspacePricingService
    (
        IWorkspacePricingRepository pricingRepository,
        IWorkspaceService workspaceService,
        CoworkingDbContext context,
        IMapper mapper
    )
    : IWorkspacePricingService
{
    public async Task<IEnumerable<WorkspacePricing>> GetPricings(WorkspacePricingQueryRequestDto request)
    {
        var pricings = await pricingRepository.GetPricings(new WorkspacePricingFilter
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

    public async Task<WorkspacePricing> CreatePricing(WorkspacePricingCreateRequestDto request)
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

        // check if workspace exists (throws)
        var workspace = await context.Workspaces.SingleOrDefaultAsync(w => w.WorkspaceId == request.WorkspaceId);
        if (workspace == null)
        {
            throw new Exception($"No workspace with '{request.WorkspaceId}' was found.");
        }

        //var workspace = await workspaceService.GetWorkspaceById(request.WorkspaceId);

        // update the latest pricing
        var latestPricing = await GetLatestPricingOfWorkspace(workspace);
        latestPricing.ValidUntil = request.ValidFrom;
        var _ = await pricingRepository.UpdatePricing(latestPricing);

        var newPricing = mapper.Map<WorkspacePricing>(request);
        return await pricingRepository.AddPricing(newPricing);
    }


    public async Task<WorkspacePricing> GetLatestPricingOfWorkspace(Workspace workspace)
    {
        var pricings = await pricingRepository.GetPricings(new WorkspacePricingFilter { WorkspaceId = workspace.WorkspaceId });

        var query = pricings.AsQueryable();

        if (!query.Any())
        {
            throw new Exception("There are no workspace pricing records.");
        }

        var latestValidFrom = await query.MaxAsync(x => x.ValidFrom);

        var latestPricing = await query
            .Where(x => x.ValidFrom == latestValidFrom)
            .SingleOrDefaultAsync()
                ??
                throw new NotFoundException($"Workspace with id '{workspace.WorkspaceId}' doesn't have a latest workspace pricing.");

        return latestPricing;
    }
}