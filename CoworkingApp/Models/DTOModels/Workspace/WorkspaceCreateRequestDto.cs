using System.ComponentModel.DataAnnotations;

namespace CoworkingApp.Models.DTOModels.Workspace;

public class WorkspaceCreateRequestDto
{
    [Required]
    public string Name { get; set; } = null!;

    [Required]
    public string Description { get; set; } = null!;

    [Required]
    public int CoworkingCenterId { get; set; }

    [Required]
    public int StatusId { get; set; }
}