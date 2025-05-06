using CoworkingApp.Models.DataModels;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CoworkingApp.Models.DtoModels
{
    [AdminDataDto]
    public class AddressDto
    {
        public int AddressId { get; set; }
        public string StreetAddress { get; set; } = null!;
        public string? District { get; set; }
        public int CityId { get; set; }
        public string? PostalCode { get; set; }
        public DateTime LastUpdated { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string City { get; set; } = null!;
        public string Country { get; set; } = null!;
    }

    [AdminResponseDto]
    public class AddressResponseDto : PaginationResponseDto
    {
        public required IEnumerable<AddressDto> Addresses { get; set; }
    }
}
