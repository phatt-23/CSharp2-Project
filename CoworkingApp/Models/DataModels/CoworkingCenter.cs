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
    [Column("coworking_center_id")]
    public int CoworkingCenterId { get; set; }

    [Column("name")]
    [StringLength(256)]
    public string Name { get; set; } = null!;

    [Column("description", TypeName = "character varying")]
    public string Description { get; set; } = null!;

    [Column("last_updated", TypeName = "timestamp without time zone")]
    public DateTime LastUpdated { get; set; }

    [Column("address_id")]
    public int AddressId { get; set; }

    [Column("updated_by")]
    public int? UpdatedBy { get; set; }

    [ForeignKey("AddressId")]
    [InverseProperty("CoworkingCenters")]
    [JsonIgnore]
    public virtual Address Address { get; set; } = null!;

    [ForeignKey("UpdatedBy")]
    [InverseProperty("CoworkingCenters")]
    [JsonIgnore]
    public virtual User? UpdatedByNavigation { get; set; }

    [InverseProperty("CoworkingCenter")]
    [JsonIgnore]
    public virtual ICollection<Workspace> Workspaces { get; set; } = new List<Workspace>();
}
