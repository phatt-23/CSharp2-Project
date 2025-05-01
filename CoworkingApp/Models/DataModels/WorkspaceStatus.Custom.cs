using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace CoworkingApp.Models.DataModels;

public partial class WorkspaceStatus
{    
    [NotMapped]
    public WorkspaceStatusType Type => Enum.GetValues<WorkspaceStatusType>().First(e => e.ToString() == Name);
}

// Description matches the name of the status in the database
// If the name in DB changes, only the description here must be changed.
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum WorkspaceStatusType
{
    [Description("Available")]
    Available,
    
    [Description("Occupied")]
    Occupied,
    
    [Description("Under Maintenance")]
    Maintenance,
}

public static class WorkspaceStatusExtensions
{
    public static WorkspaceStatusType ToWorkspaceStatusType(this WorkspaceStatus status) =>
        status.Name switch
        {
            "Available" => WorkspaceStatusType.Available,
            "Occupied" => WorkspaceStatusType.Occupied,
            "Maintenance" => WorkspaceStatusType.Maintenance,
            _ => throw new UnreachableException(),
        };

    /// Gets the status description.
    public static string ToReprString(this WorkspaceStatusType type)
    {
        var field = type.GetType().GetField(type.ToString());
        var attr = (DescriptionAttribute)Attribute.GetCustomAttribute(field!, typeof(DescriptionAttribute))!;
        return attr.Description;
    }
}