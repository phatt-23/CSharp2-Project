using System.ComponentModel.DataAnnotations.Schema;

namespace CoworkingApp.Models.DataModels;

public partial class WorkspacePricing
{
    [InverseProperty("Pricing")]
    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

    [ForeignKey("WorkspaceId")]
    [InverseProperty("WorkspacePricings")]
    public virtual Workspace Workspace { get; set; } = null!;
}