using CoworkingApp.Models.DataModels;

namespace CoworkingApp.Models.DtoModels;

public class WorkspaceHistoryDto
{
    public int WorkspaceHistoryId { get; set; }
    public int StatusId { get; set; }
    public WorkspaceStatusType Status { get; set; }
    public DateTime ChangeAt { get; set; }
}
