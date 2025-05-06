using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoworkingDesktop.Models
{
    public class Reservation
    {
        public int ReservationId { get; set; }
        public int WorkspaceId { get; set; }
        public string WorkspaceDisplayName { get; set; } = null!;
        public int CustomerId { get; set; }
        public string CustomerEmail { get; set; } = null!;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public decimal TotalPrice { get; set; }
        public int PricingId { get; set; }
        public decimal PricingPerHour { get; set; }
        public bool IsCancelled { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ReservationDto
    {
        public string WorkspaceDisplayName { get; set; } = null!;
        public string CustomerEmail { get; set; } = null!;
        public decimal PricingPerHour { get; set; } 


        public int ReservationId { get; set; }
        public int WorkspaceId { get; set; }
        public int CustomerId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public decimal TotalPrice { get; set; }
        public int PricingId { get; set; }
        public bool IsCancelled { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ReservationUpdateDto
    {
        public required int ReservationId { get; set; }
        public required int CustomerId { get; set; }
        public required int WorkspaceId { get; set; }
        public required DateTime StartTime { get; set; }
        public required DateTime EndTime { get; set; }
    }

    public class ReservationCreateDto 
    {
        public required int ReservationId { get; set; }
        public required int CustomerId { get; set; }
        public required int WorkspaceId { get; set; }
        public required DateTime StartTime { get; set; }
        public required DateTime EndTime { get; set; }
    }

    public class ReservationPageDto
    {
        public int TotalCount { get; set; }
        public List<ReservationDto> Reservations { get; set; } = null!;
    }
}
