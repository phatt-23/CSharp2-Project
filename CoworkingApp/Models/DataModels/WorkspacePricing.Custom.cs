using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CoworkingApp.Models.DataModels;

public partial class WorkspacePricing
{
    [InverseProperty("Pricing")]
    [JsonIgnore]
    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

    [ForeignKey("WorkspaceId")]
    [InverseProperty("WorkspacePricings")]
    [JsonIgnore]
    public virtual Workspace Workspace { get; set; } = null!;
}