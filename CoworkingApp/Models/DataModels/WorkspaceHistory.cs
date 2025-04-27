using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace CoworkingApp.Models.DataModels;

[Table("workspace_history")]
public partial class WorkspaceHistory
{
    [Key]
    [Column("workspace_history_id")]
    public int WorkspaceHistoryId { get; set; }

    [Column("workspace_id")]
    public int WorkspaceId { get; set; }

    [Column("status_id")]
    public int StatusId { get; set; }

    [Column("change_at", TypeName = "timestamp without time zone")]
    public DateTime ChangeAt { get; set; }

    [ForeignKey("StatusId")]
    [InverseProperty("WorkspaceHistories")]
    [JsonIgnore]
    public virtual WorkspaceStatus Status { get; set; } = null!;

    [ForeignKey("WorkspaceId")]
    [InverseProperty("WorkspaceHistories")]
    [JsonIgnore]
    public virtual Workspace Workspace { get; set; } = null!;
}
