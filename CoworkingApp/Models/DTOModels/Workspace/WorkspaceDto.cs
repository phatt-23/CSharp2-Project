namespace CoworkingApp.Models.DTOModels.Workspace;

public class WorkspaceDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public int CoworkingCenterId { get; set; }
    public int StatusId { get; set; }
    public bool IsRemoved { get; set; }
}