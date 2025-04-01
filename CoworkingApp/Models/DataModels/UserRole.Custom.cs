using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoworkingApp.Models.DataModels;

public partial class UserRole
{
    [InverseProperty("Role")]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
    
    [NotMapped]
    public UserRoleType Type => 
        Enum.GetValues<UserRoleType>().First(e => GetEnumDescription(e) == Name);

    
    /// Gets the status description.
    private static string GetEnumDescription(UserRoleType type)
    {
        var field = type.GetType().GetField(type.ToString());
        var attr = (DescriptionAttribute)Attribute.GetCustomAttribute(field!, typeof(DescriptionAttribute))!;
        return attr.Description;
    }
}

public enum UserRoleType
{
    [Description("Admin")]
    Admin,
    [Description("User")]
    User,
    [Description("Moderator")]
    Moderator,
}
