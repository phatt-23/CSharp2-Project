using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace CoworkingApp.Models.DataModels;

[Table("reservation")]
public partial class Reservation
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("workspace_id")]
    public int WorkspaceId { get; set; }

    [Column("customer_email")]
    [StringLength(256)]
    public string CustomerEmail { get; set; } = null!;

    [Column("start_time", TypeName = "timestamp without time zone")]
    public DateTime StartTime { get; set; }

    [Column("end_time", TypeName = "timestamp without time zone")]
    public DateTime EndTime { get; set; }

    [Column("price", TypeName = "money")]
    public decimal Price { get; set; }

    [Column("pricing_id")]
    public int? PricingId { get; set; }

    [ForeignKey("PricingId")]
    [InverseProperty("Reservations")]
    [JsonIgnore]
    public virtual WorkspacePricing? Pricing { get; set; }

    [ForeignKey("WorkspaceId")]
    [InverseProperty("Reservations")]
    [JsonIgnore]
    public virtual Workspace Workspace { get; set; } = null!;
}
