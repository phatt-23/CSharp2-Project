using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CoworkingApp.Models.DataModels;

public sealed partial class Workspace
{
    [ForeignKey("CoworkingCenterId")]
    [InverseProperty("Workspaces")]
    [JsonIgnore]
    public CoworkingCenter CoworkingCenter { get; set; } = null!;

    [InverseProperty("Workspace")]
    [JsonIgnore]
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

    [ForeignKey("StatusId")]
    [InverseProperty("Workspaces")]
    [JsonIgnore]
    public WorkspaceStatus Status { get; set; } = null!;

    [InverseProperty("Workspace")]
    [JsonIgnore]
    public ICollection<WorkspaceHistory> WorkspaceHistories { get; set; } = new List<WorkspaceHistory>();

    [InverseProperty("Workspace")]
    [JsonIgnore]
    public ICollection<WorkspacePricing> WorkspacePricings { get; set; } = new List<WorkspacePricing>();
}