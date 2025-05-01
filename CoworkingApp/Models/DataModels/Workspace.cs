using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace CoworkingApp.Models.DataModels;

[Table("workspace")]
public partial class Workspace
{
    [Key]
    [Column("workspace_id")]
    public int WorkspaceId { get; set; }

    [Column("name", TypeName = "character varying")]
    public string Name { get; set; } = null!;

    [Column("description", TypeName = "character varying")]
    public string Description { get; set; } = null!;

    [Column("coworking_center_id")]
    public int CoworkingCenterId { get; set; }

    [Column("is_removed")]
    public bool IsRemoved { get; set; }

    [Column("last_updated", TypeName = "timestamp without time zone")]
    public DateTime LastUpdated { get; set; }

    [Column("updated_by")]
    public int? UpdatedBy { get; set; }

    [ForeignKey("CoworkingCenterId")]
    [InverseProperty("Workspaces")]
    [JsonIgnore]
    public virtual CoworkingCenter CoworkingCenter { get; set; } = null!;

    [InverseProperty("Workspace")]
    [JsonIgnore]
    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

    [ForeignKey("UpdatedBy")]
    [InverseProperty("Workspaces")]
    [JsonIgnore]
    public virtual User? UpdatedByNavigation { get; set; }

    [InverseProperty("Workspace")]
    [JsonIgnore]
    public virtual ICollection<WorkspaceHistory> WorkspaceHistories { get; set; } = new List<WorkspaceHistory>();

    [InverseProperty("Workspace")]
    [JsonIgnore]
    public virtual ICollection<WorkspacePricing> WorkspacePricings { get; set; } = new List<WorkspacePricing>();
}
