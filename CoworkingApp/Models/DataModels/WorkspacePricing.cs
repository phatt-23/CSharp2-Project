using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace CoworkingApp.Models.DataModels;

[Table("workspace_pricing")]
public partial class WorkspacePricing
{
    [Key]
    [Column("workspace_pricing_id")]
    public int WorkspacePricingId { get; set; }

    [Column("workspace_id")]
    public int WorkspaceId { get; set; }

    /// <summary>
    /// dollar
    /// 
    /// </summary>
    [Column("price_per_hour", TypeName = "money")]
    public decimal PricePerHour { get; set; }

    [Column("valid_from", TypeName = "timestamp without time zone")]
    public DateTime ValidFrom { get; set; }

    [Column("valid_until", TypeName = "timestamp without time zone")]
    public DateTime? ValidUntil { get; set; }

    /// <summary>
    /// if null then it was created by the database admin who has direct access to the database
    /// </summary>
    [Column("created_by")]
    public int? CreatedBy { get; set; }

    [ForeignKey("CreatedBy")]
    [InverseProperty("WorkspacePricings")]
    [JsonIgnore]
    public virtual User? CreatedByNavigation { get; set; }

    [InverseProperty("Pricing")]
    [JsonIgnore]
    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

    [ForeignKey("WorkspaceId")]
    [InverseProperty("WorkspacePricings")]
    [JsonIgnore]
    public virtual Workspace Workspace { get; set; } = null!;
}
