using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CoworkingApp.Models.DataModels;

public partial class User
{
    [InverseProperty("Customer")]
    [JsonIgnore]
    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

    [ForeignKey("RoleId")]
    [InverseProperty("Users")]
    [JsonIgnore]
    public virtual UserRole Role { get; set; } = null!;
}

