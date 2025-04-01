using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace CoworkingApp.Models.DataModels;

public partial class Reservation
{
    [ForeignKey("CustomerId")]
    [InverseProperty("Reservations")]
    [JsonIgnore]
    public virtual User Customer { get; set; } = null!;
    
    
    [ForeignKey("PricingId")]
    [InverseProperty("Reservations")]
    [JsonIgnore]
    public virtual WorkspacePricing? Pricing { get; set; }

    [ForeignKey("WorkspaceId")]
    [InverseProperty("Reservations")]
    [JsonIgnore]
    public virtual Workspace Workspace { get; set; } = null!;
}
