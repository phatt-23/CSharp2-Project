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
    IMapper mapper 
    ) : ICoworkingCenterService
{
    public async Task<IEnumerable<CoworkingCenter>> GetCoworkingCentersAsync(CoworkingCenterQueryRequestDto request)
    {
        var ccs = await repository.GetCoworkingCentersAsync(new CoworkingCenterFilter
        {
            LikeName = request.LikeName,
            Latitude = request.Latitude,
            Longitude = request.Longitude
        });

        return ccs;
    }

    public async Task<CoworkingCenter> GetCoworkingCenterByIdAsync(int coworkingCenterId)
    {
        var ccs = await repository.GetCoworkingCentersAsync(
            new CoworkingCenterFilter { Id = coworkingCenterId });
        
        var cs = ccs.FirstOrDefault() ?? throw new NotFoundException($"Coworking center with {coworkingCenterId} not found");
        return cs;
    }

    public async Task<CoworkingCenter> CreateCoworkingCenterAsync(CoworkingCenterCreateRequestDto request)
    {
        var cc = await repository.AddCoworkingCenterAsync(mapper.Map<CoworkingCenter>(request));
        return cc;
    }
}