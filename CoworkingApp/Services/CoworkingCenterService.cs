using AutoMapper;
using CoworkingApp.Data;
using CoworkingApp.Models.DataModels;
using CoworkingApp.Models.DtoModels;
using CoworkingApp.Services.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Net.NetworkInformation;

namespace CoworkingApp.Services;

public interface ICoworkingCenterService
{
    Task<IEnumerable<CoworkingCenter>> GetCenters(CoworkingCenterQueryRequestDto request);
    Task<CoworkingCenter> GetCenterById(int coworkingCenterId);
    Task<CoworkingCenter> CreateCenter(CoworkingCenterCreateWithAddressRequestDto request);
    Task<CoworkingCenter> CreateCenter(CoworkingCenterCreateRequestDto request);
    Task<CoworkingCenter> UpdateCenter(int coworkingCenterId, CoworkingCenterUpdateRequestDto request);
}

public class CoworkingCenterService
    (
        IMapper mapper,
        ICoworkingCenterRepository coworkingCenterRepository,
        IAddressRepository addressRepository,
        CoworkingDbContext context,
        IGeocodingService geocodingService,
        IAddressService addressService
    ) 
    : ICoworkingCenterService
{
    public async Task<IEnumerable<CoworkingCenter>> GetCenters(CoworkingCenterQueryRequestDto request)
    {
        var centers = await coworkingCenterRepository.GetCenters(new CoworkingCenterFilter
        {
            NameContains = request.NameContains,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            IncludeAddress = true,
        });

        return centers;
    }

    public async Task<CoworkingCenter> GetCenterById(int coworkingCenterId)
    {
        var centers = await coworkingCenterRepository.GetCenters(new CoworkingCenterFilter { 
            Id = coworkingCenterId,
            IncludeAddress = true,
            IncludeWorkspaces = true, 
        });

        return centers.Single();
    }

    public async Task<CoworkingCenter> CreateCenter(CoworkingCenterCreateRequestDto request)
    {
        var address = await addressService.CreateAddressFromCoordinates(request.Latitude, request.Longitude);

        var center = mapper.Map<CoworkingCenter>(request);
        center.AddressId = address.AddressId;
        center.LastUpdated = DateTime.UtcNow;
 
        var addedCenter = (await context.CoworkingCenters.AddAsync(center)).Entity;
        await context.SaveChangesAsync();
        return await coworkingCenterRepository.GetCenterById(addedCenter.CoworkingCenterId);
    }

    public async Task<CoworkingCenter> CreateCenter(CoworkingCenterCreateWithAddressRequestDto request)
    {
        // get the geocode from the request, if it is null, throw an exception
        var geoCode = await geocodingService.Geocode(
            request.StreetAddress,
            request.District,
            request.City,
            request.PostalCode,
            request.Country)
            ?? throw new ArgumentException("The address could not be validated.");

        // find country from request
        var country = await context.Countries.FirstOrDefaultAsync(x => x.Name == request.Country);

        // if country is null, create it
        if (country == null)
        {
            var addedCountry = await context.Countries.AddAsync(new Country
            {
                Name = request.Country,
            });

            country = addedCountry.Entity;
        }

        // find city 
        var city = await context.Cities.FirstOrDefaultAsync(x => x.Name == request.City);

        // if city is null, create it
        if (city == null)
        {
            var addedCity = await context.Cities.AddAsync(new City
            {
                Name = request.City,
                Country = country,
            });

            city = addedCity.Entity;
        }

        // get the address part of the request
        var address = mapper.Map<Address>(request);

        // set up the missing properties
        address.CityId = city.CityId;
        address.Latitude = geoCode.Latitude;
        address.Longitude = geoCode.Longitude;

        // add the address to the database
        var addedAddress = await addressRepository.AddAddress(address);

        // get the coworking center part of the request
        var coworkingCenter = mapper.Map<CoworkingCenter>(request);

        // set up the missing properties
        coworkingCenter.AddressId = addedAddress.AddressId;

        // add the coworking center to the database
        var addedCenter = await coworkingCenterRepository.AddCenter(coworkingCenter);

        // return the added coworking center
        return addedCenter;
    }

    public async Task<CoworkingCenter> UpdateCenter(int coworkingCenterId, CoworkingCenterUpdateRequestDto request)
    {
        var center = mapper.Map<CoworkingCenter>(request);
        center.CoworkingCenterId = coworkingCenterId;

        return await coworkingCenterRepository.UpdateCenter(center);
    }
}
