namespace CoworkingApp.Models.DTOModels.Workspace;

public class WorkspaceCreateRequestDto
{
    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int CoworkingCenterId { get; set; }

    public int StatusId { get; set; }
}