using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CoworkingApp.Models.DataModels;

[Table("workspace_pricing")]
public partial class WorkspacePricing
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("workspace_id")]
    public int WorkspaceId { get; set; }

    [Column("price_per_hour", TypeName = "money")]
    public decimal PricePerHour { get; set; }

    [Column("valid_from", TypeName = "timestamp without time zone")]
    public DateTime ValidFrom { get; set; }

    [Column("valid_until", TypeName = "timestamp without time zone")]
    public DateTime? ValidUntil { get; set; }

    [InverseProperty("Pricing")]
    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

    [ForeignKey("WorkspaceId")]
    [InverseProperty("WorkspacePricings")]
    public virtual Workspace Workspace { get; set; } = null!;
}
