namespace CoworkingApp.Models.DTOModels.Workspace;

public class WorkspacesQueryDto : PaginationQueryDto
{
    public string? Name { get; set; }
    public int? StatusId { get; set; }
}