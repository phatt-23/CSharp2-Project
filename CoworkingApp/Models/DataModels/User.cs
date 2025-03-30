using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CoworkingApp.Models.DataModels;

[Table("user")]
[Index("Email", Name = "user_email_key", IsUnique = true)]
public partial class User
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

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
    public string? RefreshToken { get; set; } 
    
    [Column("refresh_token_expiry", TypeName = "timestamp without time zone")]
    public DateTime? RefreshTokenExpiry { get; set; }
    
    [InverseProperty("Customer")]
    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

    [ForeignKey("RoleId")]
    [InverseProperty("Users")]
    public virtual UserRole Role { get; set; } = null!;
}
