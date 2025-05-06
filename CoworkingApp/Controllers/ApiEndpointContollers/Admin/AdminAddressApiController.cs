using AutoMapper;
using CoworkingApp.Models.DtoModels;
using CoworkingApp.Services;
using CoworkingApp.Services.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CoworkingApp.Controllers.ApiEndpointContollers.Admin
{
    [ApiController]
    [Authorize(Roles = "Admin")]
    [Route("api/admin/address")]
    public class AdminAddressApiController
        (
            IAddressRepository addressRepository,
            IMapper mapper,
            IAddressService addressService
        )
        : Controller
    {
        [HttpGet]
        public async Task<ActionResult<AddressResponseDto>> GetAddresses([FromQuery] PaginationRequestDto request)
        {
            var addressFilter = new AddressFilter();
            var addresses = await addressRepository.GetAddresses(addressFilter);

            var page = Pagination.Paginate(addresses, out int total, request.PageNumber, request.PageSize);
            var dtos = mapper.Map<IEnumerable<AddressDto>>(page);

            if (dtos == null)
            {
                return BadRequest("Server side error!!!");
            }

            return Ok(new AddressResponseDto()
            {
                Addresses = dtos,
                TotalCount = total,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
            });
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<AddressDto>> GetAddressById(int id)
        {
            var addressFilter = new AddressFilter()
            {
                AddressId = id,
            };

            var addresses = await addressRepository.GetAddresses(addressFilter);
            var address = addresses.SingleOrDefault();
            if (address == null)
            {
                return BadRequest("Address with such id doesn't exist!!!");
            }

            var dto = mapper.Map<AddressDto>(address);
            if (dto == null)
            {
                return BadRequest("Server side error!!!");
            }

            return Ok(dto);
        }

        [HttpGet("coords")]
        public async Task<ActionResult<AddressDto>> GetAddressFromCoordinates([Required] decimal latitude, [Required] decimal longitude)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var created = await addressService.CreateAddressFromCoordinates(latitude, longitude);
                var addresses = await addressRepository.GetAddresses(new AddressFilter()
                {
                    AddressId = created.AddressId,
                });

                var address = addresses.SingleOrDefault();
                if (address == null)
                {
                    return BadRequest("Address with such id doesn't exist!!!");
                }

                var dto = mapper.Map<AddressDto>(address);
                if (dto == null)
                {
                    return BadRequest("Server side error!!!");
                }

                return Ok(dto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
