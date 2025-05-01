using System.Runtime.CompilerServices;

namespace CoworkingApp.Models.DataModels;

public static class ReservationExtensions
{
    public static bool HasStarted(this Reservation reservation) => reservation.StartTime <= DateTime.UtcNow;
    public static bool HasEnded(this Reservation reservation) => reservation.EndTime <= DateTime.UtcNow;
}

public static class WorkspaceExtensins
{
    public static WorkspaceHistory GetCurrentHistory(this Workspace workspace) 
    {
        return workspace.WorkspaceHistories.MaxBy(wh => wh.ChangeAt)!;
    }

    public static WorkspaceStatus GetCurrentStatus(this Workspace workspace) => workspace.GetCurrentHistory().Status;

    public static WorkspacePricing GetCurrentPricing(this Workspace workspace) =>
        workspace.WorkspacePricings.Where(wp => wp.ValidFrom <= DateTime.UtcNow).MaxBy(wp => wp.ValidFrom)!;

    public static decimal GetCurrentPricePerHour(this Workspace workspace) => workspace.GetCurrentPricing().PricePerHour;
}
    

