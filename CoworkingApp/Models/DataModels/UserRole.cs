using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace CoworkingApp.Models.DataModels;

[Table("user_role")]
[Index("Name", Name = "user_role_name_key", IsUnique = true)]
public partial class UserRole
{
    [Key]
    [Column("user_role_id")]
    public int UserRoleId { get; set; }

    [Column("name")]
    [StringLength(256)]
    public string Name { get; set; } = null!;

    [Column("description")]
    [StringLength(1024)]
    public string? Description { get; set; }

    [InverseProperty("Role")]
    [JsonIgnore]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
