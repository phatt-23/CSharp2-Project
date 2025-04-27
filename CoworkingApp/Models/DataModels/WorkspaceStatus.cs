using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace CoworkingApp.Models.DataModels;

[Table("workspace_status")]
[Index("Name", Name = "workspace_status_name_key", IsUnique = true)]
public partial class WorkspaceStatus
{
    [Key]
    [Column("workspace_status_id")]
    public int WorkspaceStatusId { get; set; }

    [Column("name")]
    [StringLength(128)]
    public string Name { get; set; } = null!;

    [Column("description", TypeName = "character varying")]
    public string Description { get; set; } = null!;

    [InverseProperty("Status")]
    [JsonIgnore]
    public virtual ICollection<WorkspaceHistory> WorkspaceHistories { get; set; } = new List<WorkspaceHistory>();
}
