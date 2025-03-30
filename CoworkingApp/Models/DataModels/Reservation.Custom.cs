using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CoworkingApp.Models.DataModels;

public partial class Reservation
{
    [ForeignKey("CustomerId")]
    [InverseProperty("Reservations")]
    public virtual User Customer { get; set; } = null!;
    
    
    [ForeignKey("PricingId")]
    [InverseProperty("Reservations")]
    public virtual WorkspacePricing? Pricing { get; set; }

    [ForeignKey("WorkspaceId")]
    [InverseProperty("Reservations")]
    public virtual Workspace Workspace { get; set; } = null!;
}
