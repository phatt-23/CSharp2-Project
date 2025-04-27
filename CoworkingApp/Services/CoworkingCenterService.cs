using AutoMapper;
using CoworkingApp.Models.DataModels;
using CoworkingApp.Models.DTOModels.CoworkingCenters;
using CoworkingApp.Services.Repositories;

namespace CoworkingApp.Services;

public interface ICoworkingCenterService
{
    Task<IEnumerable<CoworkingCenter>> GetCenters(CoworkingCenterQueryRequestDto request);
    Task<CoworkingCenter> GetCenterById(int coworkingCenterId);
    Task<CoworkingCenter> CreateCenter(CoworkingCenterCreateRequestDto request);
    Task<CoworkingCenter> UpdateCenter(int coworkingCenterId, CoworkingCenterUpdateRequestDto request);
}

public class CoworkingCenterService
    (
        ICoworkingCenterRepository repository,
        IMapper mapper 
    ) 
    : ICoworkingCenterService
{
    public async Task<IEnumerable<CoworkingCenter>> GetCenters(CoworkingCenterQueryRequestDto request)
    {
        var centers = await repository.GetCenters(new CoworkingCenterFilter
        {
            LikeName = request.LikeName,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            IncludeAddress = true,
        });

        return centers;
    }

    public async Task<CoworkingCenter> GetCenterById(int coworkingCenterId)
    {
        var centers = await repository.GetCenters(new CoworkingCenterFilter { 
            Id = coworkingCenterId,
            IncludeAddress = true,
            IncludeWorkspaces = true, 
        });

        return centers.Single();
    }

    public async Task<CoworkingCenter> CreateCenter(CoworkingCenterCreateRequestDto request)
    {
        return await repository.AddCenter(mapper.Map<CoworkingCenter>(request));
    }

    public async Task<CoworkingCenter> UpdateCenter(int coworkingCenterId, CoworkingCenterUpdateRequestDto request)
    {
        var center = mapper.Map<CoworkingCenter>(request);
        center.CoworkingCenterId = coworkingCenterId;

        return await repository.UpdateCenter(center);
    }
}
