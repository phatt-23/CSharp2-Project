namespace CoworkingApp.Models.DtoModels;

public class WorkspaceHistoryDto
{
    public int Id { get; set; }
    public int WorkspaceId { get; set; }
    public int StatusId { get; set; }
    public DateTime CreatedAt { get; set; }
}
