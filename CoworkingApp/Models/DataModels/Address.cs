using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace CoworkingApp.Models.DataModels;

[Table("address")]
public partial class Address
{
    [Key]
    [Column("address_id")]
    public int AddressId { get; set; }

    [Column("street_address")]
    [StringLength(100)]
    public string StreetAddress { get; set; } = null!;

    [Column("district")]
    [StringLength(50)]
    public string? District { get; set; }

    [Column("city_id")]
    public int CityId { get; set; }

    [Column("postal_code")]
    [StringLength(10)]
    public string? PostalCode { get; set; }

    [Column("last_updated", TypeName = "timestamp without time zone")]
    public DateTime LastUpdated { get; set; }

    [Column("latitude")]
    [Precision(9, 6)]
    public decimal Latitude { get; set; }

    [Column("longitude")]
    [Precision(9, 6)]
    public decimal Longitude { get; set; }

    [ForeignKey("CityId")]
    [InverseProperty("Addresses")]
    [JsonIgnore]
    public virtual City City { get; set; }

    [InverseProperty("Address")]
    [JsonIgnore]
    public virtual ICollection<CoworkingCenter> CoworkingCenters { get; set; } = new List<CoworkingCenter>();
}
