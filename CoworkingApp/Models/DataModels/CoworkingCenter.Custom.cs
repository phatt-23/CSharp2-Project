using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using CoworkingApp.Models.DataModels;

namespace CoworkingApp.Models.DataModels;

public partial class CoworkingCenter
{
    [InverseProperty("CoworkingCenter")]
    [JsonIgnore]
    public virtual ICollection<Workspace> Workspaces { get; set; } = [];
}

