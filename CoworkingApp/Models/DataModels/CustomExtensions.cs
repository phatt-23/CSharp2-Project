using System.Runtime.CompilerServices;

namespace CoworkingApp.Models.DataModels;

public static class ReservationExtensions
{
    public static bool HasStarted(this Reservation reservation) => reservation.StartTime <= DateTime.Now;
    public static bool HasEnded(this Reservation reservation) => reservation.EndTime <= DateTime.Now;
}
