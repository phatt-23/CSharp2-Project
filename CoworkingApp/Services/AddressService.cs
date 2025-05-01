using CoworkingApp.Data;
using CoworkingApp.Models.DataModels;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace CoworkingApp.Services;

public interface IAddressService
{
    Task<Address> CreateAddressFromCoordinates(decimal latitude, decimal longitude);
}

public class AddressService(IGeocodingService geocodingService, CoworkingDbContext context) : IAddressService
{
    public async Task<Address> CreateAddressFromCoordinates(decimal latitude, decimal longitude)
    {
        var cachedAddress = await context.Addresses.FirstOrDefaultAsync(a => a.Latitude == latitude && a.Longitude == longitude);
        if (cachedAddress != null)
            return cachedAddress;

        var reverse = await geocodingService.ReverseGeocode(latitude, longitude)
            ?? throw new InvalidOperationException($"Could not reverse geocode for Lat: {latitude}, Lng: {longitude}.");

        // check that all of the needed data is provided
        if (reverse.Country == null) 
            throw new Exception("Didn't provide country.");

        if (reverse.City == null) 
            throw new Exception("Didn't provide city.");

        if (reverse.Street == null)
            throw new Exception("Didn't provide street.");

        var country = await context.Countries.SingleOrDefaultAsync(c => c.Name == reverse.Country);

        // if country doesn't exist, add it to the database
        country ??= (await context.Countries.AddAsync(new Country
            {
                Name = reverse.Country,
            })).Entity;

        await context.SaveChangesAsync();

        var city = await context.Cities.SingleOrDefaultAsync(c => c.Name == reverse.City);


        // if city doesn't exist, add it to the database
        city ??= (await context.AddAsync(new City
            {
                Name = reverse.City,
                CountryId = country.CountryId, 
            })).Entity;

        await context.SaveChangesAsync();

        // create address record
        var address = new Address
        {
            StreetAddress = reverse.Street,
            District = reverse.District,
            CityId = city.CityId,
            PostalCode = reverse.PostalCode,
            Latitude = latitude,
            Longitude = longitude,
        };

        // check if address is already in the database
        cachedAddress = await context.Addresses.FirstOrDefaultAsync(a =>
            a.StreetAddress == address.StreetAddress &&
            a.CityId == address.CityId &&
            a.PostalCode == address.PostalCode &&
            a.District == address.District);

        // if found then return the cached address
        if (cachedAddress != null)
            return cachedAddress;

        // otherwise, add new address to the database
        address = (await context.Addresses.AddAsync(address)).Entity;
        await context.SaveChangesAsync();
        return address;
    }

}
