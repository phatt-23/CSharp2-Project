using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CoworkingApp.Models.DataModels;

public partial class WorkspaceStatus
{    
    [NotMapped]
    public WorkspaceStatusType Type => 
        Enum.GetValues<WorkspaceStatusType>().First(e => GetEnumDescription(e) == Name);

    
    /// Gets the status description.
    private static string GetEnumDescription(WorkspaceStatusType type)
    {
        var field = type.GetType().GetField(type.ToString());
        var attr = (DescriptionAttribute)Attribute.GetCustomAttribute(field!, typeof(DescriptionAttribute))!;
        return attr.Description;
    }
}

// Description matches the name of the status in the database
// If the name in DB changes, only the description here must be changed.
public enum WorkspaceStatusType
{
    [Description("Available")]
    Available,
    
    [Description("Reserved")]
    Reserved,
    
    [Description("Occupied")]
    Occupied,
    
    [Description("Under Maintenance")]
    UnderMaintenance,
}