using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoworkingDesktop.Models
{
    public class Address
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

    public class AddressResponseDto : PaginationResponseDto
    {
        public required IEnumerable<AddressDto> Addresses { get; set; }
    }
}
