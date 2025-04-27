using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace CoworkingApp.Models.DataModels;

[Table("user")]
[Index("Email", Name = "user_email_key", IsUnique = true)]
public partial class User
{
    [Key]
    [Column("user_id")]
    public int UserId { get; set; }

    [Column("email")]
    [StringLength(256)]
    public string Email { get; set; } = null!;

    [Column("password_hash", TypeName = "character varying")]
    public string PasswordHash { get; set; } = null!;

    [Column("role_id")]
    public int RoleId { get; set; }

    [Column("created_at", TypeName = "timestamp without time zone")]
    public DateTime CreatedAt { get; set; }

    [Column("refresh_token")]
    [StringLength(512)]
    public string? RefreshToken { get; set; }

    [Column("refresh_token_expiry", TypeName = "timestamp without time zone")]
    public DateTime? RefreshTokenExpiry { get; set; }

    [Column("is_removed")]
    public bool IsRemoved { get; set; }

    [InverseProperty("UpdatedByNavigation")]
    [JsonIgnore]
    public virtual ICollection<CoworkingCenter> CoworkingCenters { get; set; } = new List<CoworkingCenter>();

    [InverseProperty("Customer")]
    [JsonIgnore]
    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

    [ForeignKey("RoleId")]
    [InverseProperty("Users")]
    [JsonIgnore]
    public virtual UserRole Role { get; set; } = null!;

    [InverseProperty("CreatedByNavigation")]
    [JsonIgnore]
    public virtual ICollection<WorkspacePricing> WorkspacePricings { get; set; } = new List<WorkspacePricing>();

    [InverseProperty("UpdatedByNavigation")]
    [JsonIgnore]
    public virtual ICollection<Workspace> Workspaces { get; set; } = new List<Workspace>();
}
