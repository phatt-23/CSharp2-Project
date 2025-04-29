using CoworkingApp.Models.DataModels;

namespace CoworkingApp.Models.Misc;

public record TimelineSegment(
    int? ReservationId,
    DateTime Start,
    DateTime End,
    bool IsReserved,
    bool BelongsToUser,
    double Width);

public class TimelineData
{
    public required List<TimelineSegment> TimelineSegments { get; set; }
    public required double TotalHours { get; set; }
    public required decimal PricePerHour { get; set; }
    public required DateTime TimelineStart { get; set; }
    public required DateTime TimelineEnd { get; set; }
    public required int UserId { get; set; }
    public required IEnumerable<Reservation> Reservations { get; set; } 
    public required Workspace Workspace { get; set; }

    public static List<TimelineSegment> ComputeTimelineSegments(
        IEnumerable<Reservation> reservations,
        int userId, // User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value.TryParseToInt() ?? -1;
        out double totalHours,
        out DateTime startTime,
        out DateTime endTime)
    {
        DateTime maxDateTime(DateTime a, DateTime b) => (a > b) ? a : b;
        DateTime minDateTime(DateTime a, DateTime b) => (a < b) ? a : b;

        // the time span of the timeline (max 1 year ahead)
        var timelineStartTime = startTime = DateTime.Now;
        var timelineEndTime = endTime = minDateTime(reservations.Where(x => !x.IsCancelled).Max(x => x.EndTime), DateTime.Today.AddYears(1));

        totalHours = (timelineEndTime - timelineStartTime).TotalHours;

        // sorted reservations by time, filtered by timeline span

        // build segments of free and reserved times
        var segments = new List<TimelineSegment>();

        var timeCursor = timelineStartTime;

        foreach (var reservation in reservations
            .AsQueryable()
            .OrderBy(r => r.StartTime)
            .Where(x => !x.IsCancelled && timelineStartTime <= x.EndTime && x.StartTime <= timelineEndTime).ToList())
        {
            var reservationBelongsToUser = reservation.CustomerId == userId;

            if (timeCursor < reservation.StartTime)
            {
                segments.Add(new TimelineSegment(
                    ReservationId: reservation.ReservationId,
                    Start: timeCursor,
                    End: maxDateTime(timelineStartTime, reservation.StartTime),
                    IsReserved: false,
                    BelongsToUser: reservationBelongsToUser,
                    Width: (maxDateTime(timelineStartTime, reservation.StartTime) - timeCursor).TotalHours / totalHours));
            }

            // this reserved segment
            segments.Add(new TimelineSegment(
                ReservationId: reservation.ReservationId,
                Start: maxDateTime(timelineStartTime, reservation.StartTime),
                End: reservation.EndTime,
                IsReserved: true,
                BelongsToUser: reservationBelongsToUser,
                Width: (reservation.EndTime - maxDateTime(timelineStartTime, reservation.StartTime)).TotalHours / totalHours));

            // next
            timeCursor = reservation.EndTime;
        }

        // free after last reservation?
        if (timeCursor < timelineEndTime)
        {
            segments.Add(new TimelineSegment(
                ReservationId: null,
                Start: timeCursor,
                End: timelineEndTime,
                IsReserved: false,
                BelongsToUser: false,
                Width: (timelineEndTime - timeCursor).TotalHours / totalHours));
        }

        return segments;
    }
}