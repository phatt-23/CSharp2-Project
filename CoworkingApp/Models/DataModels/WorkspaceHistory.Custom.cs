using System.ComponentModel.DataAnnotations.Schema;

namespace CoworkingApp.Models.DataModels;

public sealed partial class WorkspaceHistory
{
    [ForeignKey("StatusId")]
    [InverseProperty("WorkspaceHistories")]
    public WorkspaceStatus Status { get; set; } = null!;

    [ForeignKey("WorkspaceId")]
    [InverseProperty("WorkspaceHistories")]
    public Workspace Workspace { get; set; } = null!;
}