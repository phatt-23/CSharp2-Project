using AutoMapper;
using CoworkingApp.Models.DtoModels;
using CoworkingApp.Services;
using CoworkingApp.Types;
using Microsoft.AspNetCore.Mvc;

namespace CoworkingApp.Controllers.ApiEndpointContollers.Public;

public interface IPricingApi
{
    Task<ActionResult<IEnumerable<WorkspacePricingDto>>> GetPricingsOfWorkspaceByIdAsync(int workspaceId);
}

[PublicApiController]
[Route("api/workspace-pricing")]
public class WorkspacePricingApiController
    (
        IWorkspacePricingService pricingService,
        IMapper mapper
    ) 
    : Controller, IPricingApi
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<WorkspacePricingDto>>> GetPricingsOfWorkspaceByIdAsync(int id)
    {
        var pricings = await pricingService.GetPricings(new WorkspacePricingQueryRequestDto 
        { 
            WorkspaceId = id 
        });
        
        var pricingDtos = mapper.Map<IEnumerable<WorkspacePricingDto>>(pricings);
        
        return Ok(pricingDtos);
    }
}
