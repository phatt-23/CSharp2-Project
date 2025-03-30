using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CoworkingApp.Models.DataModels;

[Table("workspace_history")]
public partial class WorkspaceHistory
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("workspace_id")]
    public int WorkspaceId { get; set; }

    [Column("status_id")]
    public int StatusId { get; set; }

    [Column("created_at", TypeName = "timestamp without time zone")]
    public DateTime CreatedAt { get; set; }
}
