using AutoMapper;
using CoworkingApp.Data;
using CoworkingApp.Models.DataModels;
using CoworkingApp.Models.DTOModels.CoworkingCenters;
using CoworkingApp.Models.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace CoworkingApp.Services;

public class CoworkingCentersService(
    CoworkingDbContext context,
    IMapper mapper 
    )
{
    public async Task<(IEnumerable<CoworkingCenterDto>, int)> GetAsync(CoworkingCentersQueryDto query)
    {
        var coworkingCenters = context.CoworkingCenters.AsQueryable();

        if (query.Name != null)
            coworkingCenters = coworkingCenters.Where(c => c.Name == query.Name);
        if (query.Latitude.HasValue)
            coworkingCenters = coworkingCenters.Where(c => c.Latitude == query.Latitude.Value);
        if (query.Longitude.HasValue)
            coworkingCenters = coworkingCenters.Where(c => c.Longitude == query.Longitude.Value);
        if (query.LatitudeLow.HasValue)
            coworkingCenters = coworkingCenters.Where(c => c.Latitude >= query.LatitudeLow.Value); 
        if (query.LatitudeHigh.HasValue)
            coworkingCenters = coworkingCenters.Where(c => c.Latitude <= query.LatitudeHigh.Value);
        if (query.LongitudeLow.HasValue)
            coworkingCenters = coworkingCenters.Where(c => c.Longitude >= query.LongitudeLow.Value);
        if (query.LongitudeHigh.HasValue)
            coworkingCenters = coworkingCenters.Where(c => c.Longitude <= query.LongitudeHigh.Value);
        
        var totalCount = await coworkingCenters.CountAsync();

        coworkingCenters = coworkingCenters
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .Include(cc => cc.Workspaces);
        
        var coworkingCenterDtos = mapper.Map<IEnumerable<CoworkingCenterDto>>(coworkingCenters);
        
        return (coworkingCenterDtos, totalCount);
    }
    
    public async Task<CoworkingCenterDto> GetByIdAsync(int id)
    {
        var coworkingCenter = await context.CoworkingCenters
            .Where(cc => cc.Id == id)
            .Include(cc => cc.Workspaces)
            .SingleAsync();

        if (coworkingCenter == null)
            throw new NotFoundException($"Coworking center with {id} not found");

        return mapper.Map<CoworkingCenterDto>(coworkingCenter);
    }

    public async Task<CoworkingCenterDto> CreateAsync(CoworkingCenterCreateRequestDto request)
    {
        // create and save to database
        throw new NotImplementedException();
    }
}