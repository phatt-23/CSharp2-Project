using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoworkingDesktop.Models
{
    public class Pricing
    {
        public int Id { get; set; }
        public int WorkspaceId { get; set; }
        public decimal PricePerHour { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime? ValidUntil { get; set; }
    }

    public class PricingDto
    {
        public int Id { get; set; }
        public int WorkspaceId { get; set; }
        public decimal PricePerHour { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime? ValidUntil { get; set; }
    }

    public class PricingCreateDto
    {
        public required int WorkspaceId { get; set; }
        public required decimal PricePerHour { get; set; }
        public required DateTime ValidFrom { get; set; }
        public required DateTime? ValidUntil { get; set; }
    }
}
