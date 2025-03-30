using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CoworkingApp.Models.DataModels;

[Table("workspace")]
public partial class Workspace
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name", TypeName = "character varying")]
    public string Name { get; set; } = null!;

    [Column("description", TypeName = "character varying")]
    public string Description { get; set; } = null!;

    [Column("coworking_center_id")]
    public int CoworkingCenterId { get; set; }

    [Column("status_id")]
    public int StatusId { get; set; }

    [Column("is_removed")]
    public bool IsRemoved { get; set; }

    [Column("created_at", TypeName = "timestamp without time zone")]
    public DateTime CreatedAt { get; set; }
}
