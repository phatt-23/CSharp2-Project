using CoworkingApp.Models.DataModels;
using CoworkingApp.Models.DTOModels.WorkspaceHistory;

namespace CoworkingApp.Models.DTOModels.Workspace;

public class WorkspaceHistoriesResponseDto
{
    public required WorkspaceDto Workspace { get; set; }
    public required ICollection<WorkspaceHistoryDto> Histories { get; set; }
}