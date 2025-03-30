namespace CoworkingApp.Models.DTOModels.Workspace;

public class WorkspacesResponseDto : PaginationResponseDto
{
    public required IEnumerable<WorkspaceDto> Workspaces { get; set; }
}