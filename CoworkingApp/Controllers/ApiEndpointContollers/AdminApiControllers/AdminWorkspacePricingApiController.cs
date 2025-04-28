using AutoMapper;
using CoworkingApp.Models.DtoModels;
using CoworkingApp.Services;
using CoworkingApp.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoworkingApp.Controllers.APIEndpoints.Admin;

public interface IAdminWorkspacePricingApi
{
    Task<ActionResult<IEnumerable<AdminWorkspacePricingDto>>> GetWorkspacePricingsAsync([FromQuery] WorkspacePricingQueryRequestDto request);
    Task<ActionResult<AdminWorkspacePricingDto>> CreateWorkspacePricingAsync([FromBody] WorkspacePricingCreateRequestDto request);
}

[ApiController]
[AdminApiController]
[Route("api/admin/pricing/")]
public class AdminWorkspacePricingApiController
    (
        IWorkspacePricingService pricingService,
        IMapper mapper
    ) 
    : Controller, IAdminWorkspacePricingApi
{
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async 
        Task<ActionResult<IEnumerable<AdminWorkspacePricingDto>>> 
        GetWorkspacePricingsAsync([FromQuery] WorkspacePricingQueryRequestDto request)
    {
        try
        {
            var pricings = await pricingService.GetPricings(request);
            var pricingDtos = mapper.Map<IEnumerable<WorkspacePricingDto>>(pricings);
            return Ok(pricingDtos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<AdminWorkspacePricingDto>> CreateWorkspacePricingAsync([FromBody] WorkspacePricingCreateRequestDto request)
    {
        try
        {
            var pricing = await pricingService.CreatePricing(request);
            return Ok(pricing);
        }
        catch (Exception e)
        {
            return StatusCode(500, e);
        }
    }
}