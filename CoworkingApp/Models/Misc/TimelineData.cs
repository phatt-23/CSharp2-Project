using CoworkingApp.Models.DataModels;

namespace CoworkingApp.Models.Misc;

public record TimelineSegment(
    int? ReservationId,
    DateTime Start,
    DateTime End,
    bool IsReserved,
    bool BelongsToUser,
    double Width,
    WorkspaceStatusType Status
);

public class TimelineData
{
    public List<TimelineSegment> Segments { get; set; }
    public double TotalHours { get; set; }
    public DateTime TimelineStart { get; set; }
    public DateTime TimelineEnd { get; set; }
    public int UserId { get; set; }
    public IEnumerable<Reservation> Reservations { get; set; } 
    public Workspace Workspace { get; set; }

    // workspace with included histories and then status
    public TimelineData(Workspace workspace, IEnumerable<Reservation> reservations, int userId)
    {
        DateTime maxDateTime(DateTime a, DateTime b) => (a > b) ? a : b;
        DateTime minDateTime(DateTime a, DateTime b) => (a < b) ? a : b;

        // primitive
        this.Workspace = workspace;
        this.Reservations = reservations;
        this.UserId = userId;

        // the time span of the timeline (max 1 year ahead)
        this.TimelineStart = DateTime.Now;
        this.TimelineEnd = minDateTime(
            reservations
                .Where(r => !r.IsCancelled && r.EndTime >= this.TimelineStart)
                .MaxBy(r => r.EndTime)?.EndTime ?? DateTime.Today.AddYears(1), 
            DateTime.Today.AddYears(1)
        );

        this.TotalHours = (this.TimelineEnd - this.TimelineStart).TotalHours;

        // sorted reservations by time, filtered by timeline span

        // build segments of free and reserved times
        this.Segments = [];

        var timeCursor = this.TimelineStart;

        foreach (var reservation in reservations
            .AsQueryable()
            .Where(x => !x.IsCancelled && this.TimelineStart <= x.EndTime && x.StartTime <= this.TimelineEnd)
            .OrderBy(r => r.StartTime)
            .ToList())
        {
            var reservationBelongsToUser = reservation.CustomerId == userId;

            if (timeCursor < reservation.StartTime)
            {
                this.Segments.Add(new TimelineSegment(
                    ReservationId: reservation.ReservationId,
                    Start: timeCursor,
                    End: maxDateTime(this.TimelineStart, reservation.StartTime),
                    IsReserved: false,
                    BelongsToUser: reservationBelongsToUser,
                    Width: (maxDateTime(this.TimelineStart, reservation.StartTime) - timeCursor).TotalHours / this.TotalHours,
                    Status: workspace.GetCurrentStatus().Type));
            }

            // this reserved segment
            this.Segments.Add(new TimelineSegment(
                ReservationId: reservation.ReservationId,
                Start: maxDateTime(this.TimelineStart, reservation.StartTime),
                End: reservation.EndTime,
                IsReserved: true,
                BelongsToUser: reservationBelongsToUser,
                Width: (reservation.EndTime - maxDateTime(this.TimelineStart, reservation.StartTime)).TotalHours / this.TotalHours,
                Status: workspace.GetCurrentStatus().Type));

            // next
            timeCursor = reservation.EndTime;
        }

        // free after last reservation?
        if (timeCursor < this.TimelineEnd)
        {
            this.Segments.Add(new TimelineSegment(
                ReservationId: null,
                Start: timeCursor,
                End: this.TimelineEnd,
                IsReserved: false,
                BelongsToUser: false,
                Width: (this.TimelineEnd - timeCursor).TotalHours / this.TotalHours,
                Status: workspace.GetCurrentStatus().Type));
        }
    }
}