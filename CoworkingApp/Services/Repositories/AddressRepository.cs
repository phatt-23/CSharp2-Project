
using AutoFilterer.Attributes;
using AutoFilterer.Enums;
using AutoFilterer.Types;
using AutoFilterer.Extensions;
using CoworkingApp.Data;
using CoworkingApp.Models.DataModels;
using Microsoft.EntityFrameworkCore;

namespace CoworkingApp.Services.Repositories;

public interface IAddressRepository 
{
    Task<Address> AddAddress(Address address);
    Task<IEnumerable<Address>> GetAddresses(AddressFilter filter);
}

public class AddressRepository
    (
        CoworkingDbContext context
    ) 
    : IAddressRepository
{
    public async Task<Address> AddAddress(Address address)
    {
        if (address.City == null)
        {
            throw new ArgumentNullException(nameof(address.City), "City cannot be null");
        }

        if (address.City.Country == null)
        {
            throw new ArgumentNullException(nameof(address.City.Country), "Country cannot be null");
        }

        var addedAddress = await context.Addresses.AddAsync(address);
        await context.SaveChangesAsync();
        return addedAddress.Entity;
    }

    public async Task<IEnumerable<Address>> GetAddresses(AddressFilter filter)
    {
        var query = context.Addresses.ApplyFilter(filter);

        query = filter.Latitude.ApplyTo(query, a => a.Latitude);
        query = filter.Longitude.ApplyTo(query, a => a.Longitude);

        query = query
            .Include(address => address.City)
            .ThenInclude(city => city.Country);

        return await query.ToListAsync();
    }
}

public class AddressFilter : FilterBase
{
    [CompareTo(nameof(Address.AddressId))]
    public int? AddressId { get; set; }

    [CompareTo(nameof(Address.StreetAddress))]
    [StringFilterOptions(StringFilterOption.Contains)]
    public string? StreetAddressContains { get; set; }

    [CompareTo(nameof(Address.District))]
    [StringFilterOptions(StringFilterOption.Contains)]
    public string? DistrictContains { get; set; }

    [CompareTo(nameof(Address.CityId))]
    public int? CityId { get; set; }

    [CompareTo(nameof(Address.PostalCode))]
    public string? PostalCode { get; set; }

    public RangeFilter<decimal> Latitude { get; set; } = new();
    public RangeFilter<decimal> Longitude { get; set; } = new();
}