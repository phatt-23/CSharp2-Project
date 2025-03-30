using CoworkingApp.Models.DTOModels.User;
using CoworkingApp.Models.DTOModels.Workspace;

namespace CoworkingApp.Models.DTOModels.Reservation;

// Data DTOs

public class ReservationDto
{
    public int Id { get; set; }
    public int WorkspaceId { get; set; }
    public int CustomerId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public decimal TotalPrice { get; set; }
    public int? PricingId { get; set; }
}


[AdminDto]
public class AdminReservationDto : ReservationDto
{
    public bool IsCancelled { get; set; }
    
    public virtual UserDto Customer { get; set; } = null!;
    
    public virtual WorkspacePricingDto? Pricing { get; set; }

    public virtual AdminWorkspaceDto Workspace { get; set; } = null!;
}