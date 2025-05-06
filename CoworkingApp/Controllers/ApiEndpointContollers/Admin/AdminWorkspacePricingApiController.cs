using AutoMapper;
using CoworkingApp.Models.DtoModels;
using CoworkingApp.Services;
using CoworkingApp.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoworkingApp.Controllers.ApiEndpointContollers.AdminApiControllers;

public interface IAdminWorkspacePricingApi
{
    Task<ActionResult<IEnumerable<AdminWorkspacePricingDto>>> GetWorkspacePricingsAsync([FromQuery] WorkspacePricingQueryRequestDto request);
    Task<ActionResult<AdminWorkspacePricingDto>> CreateWorkspacePricingAsync([FromBody] WorkspacePricingCreateRequestDto request);
}

[AdminApiController]
[Route("api/admin/pricing/")]
[Authorize(Roles = "Admin")]
public class AdminWorkspacePricingApiController
    (
        IWorkspacePricingService pricingService,
        IMapper mapper
    ) 
    : Controller, IAdminWorkspacePricingApi
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AdminWorkspacePricingDto>>> GetWorkspacePricingsAsync([FromQuery] WorkspacePricingQueryRequestDto request)
    {
        try
        {
            var pricings = await pricingService.GetPricings(request);
            var pricingDtos = mapper.Map<IEnumerable<WorkspacePricingDto>>(pricings);
            return Ok(pricingDtos);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("{id:int}")]

    public async Task<ActionResult<AdminWorkspacePricingDto>> GetWorkspacePricingAsync(int id)
    {
        try
        {
            var pricing = await pricingService.GetPricingById(id);
            var pricingDto = mapper.Map<WorkspacePricingDto>(pricing);
            return Ok(pricingDto);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult<AdminWorkspacePricingDto>> CreateWorkspacePricingAsync([FromBody] WorkspacePricingCreateRequestDto request)
    {
        try
        {
            var pricing = await pricingService.CreatePricing(request);
            return Ok(pricing);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }
}