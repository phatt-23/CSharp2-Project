
namespace CoworkingApp.Models.DTOModels.Workspace;

////////////////////////////////////////////////////////////
// DTOs representing responses (that hold data DTOs)
////////////////////////////////////////////////////////////

 
public class WorkspacesResponseDto : PaginationResponseDto
{
    public required IEnumerable<WorkspaceDto> Workspaces { get; set; }
}


[AdminDto]
public class AdminWorkspacesResponseDTo : PaginationResponseDto
{
    public required IEnumerable<AdminWorkspaceDto> Workspaces { get; set; }
}


[AdminDto]
public class WorkspaceHistoriesResponseDto
{
    public required WorkspaceDto Workspace { get; set; }
    public required IEnumerable<WorkspaceHistoryDto> Histories { get; set; }
}


