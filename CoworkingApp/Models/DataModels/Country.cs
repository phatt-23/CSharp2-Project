using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace CoworkingApp.Models.DataModels;

[Table("country")]
[Index("Name", Name = "unique_country_name", IsUnique = true)]
public partial class Country
{
    [Key]
    [Column("country_id")]
    public int CountryId { get; set; }

    [Column("name")]
    [StringLength(50)]
    public string Name { get; set; } = null!;

    [Column("last_updated", TypeName = "timestamp without time zone")]
    public DateTime LastUpdated { get; set; }

    [InverseProperty("Country")]
    [JsonIgnore]
    public virtual ICollection<City> Cities { get; set; } = new List<City>();
}
