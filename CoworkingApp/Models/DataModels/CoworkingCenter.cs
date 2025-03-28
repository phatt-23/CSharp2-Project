using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace CoworkingApp.Models.DataModels;

[Table("coworking_center")]
public partial class CoworkingCenter
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    [StringLength(256)]
    public string Name { get; set; } = null!;

    [Column("description", TypeName = "character varying")]
    public string Description { get; set; } = null!;

    [Column("latitude")]
    [Precision(9, 6)]
    public decimal Latitude { get; set; }

    [Column("longitude")]
    [Precision(9, 6)]
    public decimal Longitude { get; set; }

    [InverseProperty("CoworkingCenter")]
    [JsonIgnore]
    public virtual ICollection<Workspace> Workspaces { get; set; } = new List<Workspace>();
}
