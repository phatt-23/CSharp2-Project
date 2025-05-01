using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CoworkingApp.Models.DataModels;

[Table("city")]
public partial class City
{
    [Key]
    [Column("city_id")]
    public int CityId { get; set; }

    [Column("name")]
    [StringLength(50)]
    public string Name { get; set; } = null!;

    [Column("country_id")]
    public int CountryId { get; set; }

    [Column("last_updated", TypeName = "timestamp without time zone")]
    public DateTime LastUpdated { get; set; }

    [InverseProperty("City")]
    [JsonIgnore]
    public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();

    [ForeignKey("CountryId")]
    [InverseProperty("Cities")]
    [JsonIgnore]
    public virtual Country Country { get; set; } = null!;
}
