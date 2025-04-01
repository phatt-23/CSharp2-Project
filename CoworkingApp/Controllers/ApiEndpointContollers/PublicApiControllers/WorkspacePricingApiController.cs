using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace CoworkingApp.Controllers.APIEndpoints.Public;

public interface IPricingApi
{
    Task<ActionResult<IEnumerable<WorkspacePricingDto>>> GetPricingsOfWorkspaceByIdAsync(int workspaceId);
}

[ApiController]
[Route("api/workspace-pricing")]
public class WorkspacePricingApiController(
    IWorkspacePricingService pricingService,
    IMapper mapper
    ) : Controller, IPricingApi
{
    [HttpGet("{id:int}")]
    public async Task<ActionResult<IEnumerable<WorkspacePricingDto>>> GetPricingsOfWorkspaceByIdAsync(int id)
    {
        var pricings = await pricingService.GetPricingsAsync(
            new WorkspacePricingQueryRequestDto { WorkspaceId = id });
        
        var pricingDtos = mapper.Map<IEnumerable<WorkspacePricingDto>>(pricings);
        
        return Ok(pricingDtos);
    }
}
