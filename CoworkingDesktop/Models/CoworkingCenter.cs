using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoworkingDesktop.Models
{
    public class CoworkingCenter
    {
        public int CoworkingCenterId { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int AddressId { get; set; }
        public string AddressDisplayName { get; set; } = null!;
        public DateTime LastUpdated { get; set; }
        public int? UpdatedBy { get; set; }
    }

    public class CoworkingCenterDto
    {
        public int CoworkingCenterId { get; set; } 
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int AddressId { get; set; } 
        public string AddressDisplayName { get; set; } = null!; 
        public DateTime LastUpdated { get; set; } 
        public int? UpdatedBy { get; set; } 
    }

    public class CoworkingCenterCreateDto
    {
        public required string Name { get; set; } = null!;
        public required string Description { get; set; } = string.Empty;
        public required decimal Latitude { get; set; }
        public required decimal Longitude { get; set; }
    }

    public class CoworkingCenterUpdateDto
    {
        public required int CoworkingCenterId { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required decimal Latitude { get; set; }
        public required decimal Longitude { get; set; }
    }

    public class CoworkingCenterResponseDto : PaginationResponseDto
    {
        public required List<CoworkingCenterDto> Centers { get; set; }
    }

}
