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
    [Column("reservation_id")]
    public int ReservationId { get; set; }

    [Column("workspace_id")]
    public int WorkspaceId { get; set; }

    [Column("start_time", TypeName = "timestamp without time zone")]
    public DateTime StartTime { get; set; }

    [Column("end_time", TypeName = "timestamp without time zone")]
    public DateTime EndTime { get; set; }

    [Column("total_price", TypeName = "money")]
    public decimal? TotalPrice { get; set; }

    [Column("pricing_id")]
    public int PricingId { get; set; }

    [Column("created_at", TypeName = "timestamp without time zone")]
    public DateTime CreatedAt { get; set; }

    [Column("customer_id")]
    public int? CustomerId { get; set; }

    [Column("is_cancelled")]
    public bool IsCancelled { get; set; }

    [ForeignKey("CustomerId")]
    [InverseProperty("Reservations")]
    [JsonIgnore]
    public virtual User? Customer { get; set; }

    [ForeignKey("PricingId")]
    [InverseProperty("Reservations")]
    [JsonIgnore]
    public virtual WorkspacePricing Pricing { get; set; } = null!;

    [ForeignKey("WorkspaceId")]
    [InverseProperty("Reservations")]
    [JsonIgnore]
    public virtual Workspace Workspace { get; set; } = null!;

    [InverseProperty("Reservation")]
    [JsonIgnore]
    public virtual ICollection<WorkspaceHistory> WorkspaceHistories { get; set; } = new List<WorkspaceHistory>();
}
