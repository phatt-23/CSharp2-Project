namespace CoworkingApp.Models.DTOModels.WorkspaceStatus;

public class WorkspaceStatusesResponseDto
{
    public required int TotalCount { get; set; }
    public required ICollection<WorkspaceStatusDto> WorkspaceStatuses { get; set; }
}