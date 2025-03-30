using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoworkingApp.Models.DataModels;

public partial class UserRole
{
    [InverseProperty("Role")]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}

public enum UserRoleType
{
    Admin,
    User,
    Manager,
}
