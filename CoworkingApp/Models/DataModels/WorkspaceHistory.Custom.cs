using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CoworkingApp.Models.DataModels;

public sealed partial class WorkspaceHistory
{
    [ForeignKey("StatusId")]
    [InverseProperty("WorkspaceHistories")]
    [JsonIgnore]
    public WorkspaceStatus Status { get; set; } = null!;

    [ForeignKey("WorkspaceId")]
    [InverseProperty("WorkspaceHistories")]
    [JsonIgnore]
    public Workspace Workspace { get; set; } = null!;
}