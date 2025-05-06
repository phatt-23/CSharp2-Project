using System.Diagnostics;
using System.Text.Json.Serialization;

namespace CoworkingApp.Models.DtoModels;


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

// Public

[PublicDto]
public class WorkspaceReservationCountDto
{
    public required int WorkspaceId { get; set; }
    public required int CoworkingCenterId { get; set; }
    public required int ReservationCount { get; set; }
}

[PublicDto]
public class WorkspaceReservationCountsResponseDto
{
    public required TimeBack TimeBack { get; set; }
    public required ICollection<WorkspaceReservationCountDto> ReservationCounts { get; set; }
}

[PublicDto]
public class CoworkingCenterReservationCountDto
{
    public required int CoworkingCenterId { get; set; }
    public required int ReservationCount { get; set; }
}

[PublicDto]
public class CoworkingCenterReservationCountsResponseDto
{
    public required TimeBack TimeBack { get; set; }
    public required ICollection<CoworkingCenterReservationCountDto> ReservationCounts { get; set; }
}

// Admin

[AdminDataDto]
public class WorkspaceRevenueDto
{
    public required int WorkspaceId { get; set; }
    public required string WorkspaceDisplayName { get; set; }
    public required int CoworkingCenterId { get; set; }
    public required decimal Revenue { get; set; }
    public required List<int> FinishedReservations { get; set; }
}

[AdminResponseDto]
public class WorkspaceRevenuesResponseDto
{
    public required TimeBack TimeBack { get; set; }
    public required ICollection<WorkspaceRevenueDto> Revenues { get; set; }
}

[AdminResponseDto]
public class WorkspaceRevenueResponseDto
{
    public required TimeBack TimeBack { get; set; }
    public required WorkspaceRevenueDto Revenue { get; set; }
}


[AdminDataDto]
public class CoworkingCenterRevenueDto
{
    public required int CoworkingCenterId { get; set; }
    public required string CoworkingCenterDisplayName { get; set; }
    public required decimal Revenue { get; set; }
    public required List<WorkspaceRevenueDto> Revenues { get; set; }
}

[AdminResponseDto]
public class CoworkingCenterRevenuesResponseDto
{
    public required TimeBack TimeBack { get; set; }
    public required ICollection<CoworkingCenterRevenueDto> Revenues { get; set; }
}

[AdminResponseDto]
public class CoworkingCenterRevenueResponseDto
{
    public required TimeBack TimeBack { get; set; }
    public required CoworkingCenterRevenueDto Revenue { get; set; }
}