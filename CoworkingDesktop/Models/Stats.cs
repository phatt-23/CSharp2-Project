using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CoworkingDesktop.Models
{
    public class WorkspaceRevenueDto
    {
        public required int WorkspaceId { get; set; }
        public required string WorkspaceDisplayName { get; set; }
        public required int CoworkingCenterId { get; set; }
        public required decimal Revenue { get; set; }
        public required List<int> FinishedReservations { get; set; }
    }

    public class WorkspaceRevenuesResponseDto
    {
        public required TimeBack TimeBack { get; set; }
        public required List<WorkspaceRevenueDto> Revenues { get; set; }
    }

    public class WorkspaceRevenueResponseDto
    {
        public required TimeBack TimeBack { get; set; }
        public required WorkspaceRevenueDto Revenue { get; set; }
    }


    public class CoworkingCenterRevenueDto
    {
        public required int CoworkingCenterId { get; set; }
        public required string CoworkingCenterDisplayName { get; set; }
        public required decimal Revenue { get; set; }
        public required List<WorkspaceRevenueDto> Revenues { get; set; }
    }

    public class CoworkingCenterRevenuesResponseDto
    {
        public required TimeBack TimeBack { get; set; }
        public required List<CoworkingCenterRevenueDto> Revenues { get; set; }
    }

    public class CoworkingCenterRevenueResponseDto
    {
        public required TimeBack TimeBack { get; set; }
        public required CoworkingCenterRevenueDto Revenue { get; set; }
    }


    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TimeBack
    {
        LastWeek,
        LastMonth,
        LastYear,
        Unbound,
    }

    public static class TimeBackExtensions
    {
        public static DateTime ToDateTime(this TimeBack timeBack) => timeBack switch
        {
            TimeBack.LastWeek => DateTime.Now.AddDays(-7),
            TimeBack.LastMonth => DateTime.Now.AddMonths(-1),
            TimeBack.LastYear => DateTime.Now.AddYears(-1),
            TimeBack.Unbound => new DateTime(0),
            _ => throw new UnreachableException()
        };
    }
}
