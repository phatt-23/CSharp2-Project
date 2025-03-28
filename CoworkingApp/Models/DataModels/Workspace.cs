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
    [Column("id")]
    public int Id { get; set; }

    [Column("name", TypeName = "character varying")]
    public string Name { get; set; } = null!;

    [Column("description", TypeName = "character varying")]
    public string Description { get; set; } = null!;

    [Column("coworking_center_id")]
    public int CoworkingCenterId { get; set; }

    [Column("status_id")]
    public int StatusId { get; set; }

    [ForeignKey("CoworkingCenterId")]
    [InverseProperty("Workspaces")]
    [JsonIgnore]
    public virtual CoworkingCenter CoworkingCenter { get; set; } = null!;

    [InverseProperty("Workspace")]
    [JsonIgnore]
    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

    [ForeignKey("StatusId")]
    [InverseProperty("Workspaces")]
    [JsonIgnore]
    public virtual WorkspaceStatus Status { get; set; } = null!;

    [InverseProperty("Workspace")]
    [JsonIgnore]
    public virtual ICollection<WorkspaceHistory> WorkspaceHistories { get; set; } = new List<WorkspaceHistory>();

    [InverseProperty("Workspace")]
    [JsonIgnore]
    public virtual ICollection<WorkspacePricing> WorkspacePricings { get; set; } = new List<WorkspacePricing>();
}
