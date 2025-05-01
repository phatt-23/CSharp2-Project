using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CoworkingApp.Models.DataModels;

public partial class UserRole
{    
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

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum UserRoleType
{
    [Description("Admin")]
    Admin,
    [Description("Customer")]
    Customer,
    [Description("Moderator")]
    Moderator,
}
