using AutoMapper;
using CoworkingApp.Data;
using CoworkingApp.Models.DataModels;
using CoworkingApp.Models.DTOModels.CoworkingCenters;
using CoworkingApp.Models.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace CoworkingApp.Services;

public interface ICoworkingCenterService
{
    Task<IEnumerable<CoworkingCenter>> GetCoworkingCentersAsync(CoworkingCenterQueryRequestDto request);
    Task<CoworkingCenter> GetCoworkingCenterByIdAsync(int coworkingCenterId);
    Task<CoworkingCenter> CreateCoworkingCenterAsync(CoworkingCenterCreateRequestDto request);
}

public class CoworkingCenterService(
    ICoworkingCenterRepository repository,
    CoworkingDbContext context,
    IMapper mapper 
    ) : ICoworkingCenterService
{
    public async Task<IEnumerable<CoworkingCenter>> GetCoworkingCentersAsync(CoworkingCenterQueryRequestDto request)
    {
        var ccs = await repository.GetCoworkingCentersAsync(new CoworkingCenterFilterOptions
        {
            LikeName = request.Name,
            LatitudeLow = request.LatitudeLow,
            LatitudeHigh = request.LatitudeHigh,
            LongitudeLow = request.LatitudeLow,
            LongitudeHigh = request.LongitudeHigh
        });

        return ccs;
    }

    public async Task<CoworkingCenter> GetCoworkingCenterByIdAsync(int coworkingCenterId)
    {
        var ccs = await repository.GetCoworkingCentersAsync(
            new CoworkingCenterFilterOptions { Id = coworkingCenterId });
        
        var cs = ccs.FirstOrDefault();
        if (cs == null) 
            throw new NotFoundException($"Coworking center with {coworkingCenterId} not found");
        
        return cs;
    }

    public async Task<CoworkingCenter> CreateCoworkingCenterAsync(CoworkingCenterCreateRequestDto request)
    {
        var cc = await repository.AddCoworkingCenterAsync(mapper.Map<CoworkingCenter>(request));
        return cc;
    }
}