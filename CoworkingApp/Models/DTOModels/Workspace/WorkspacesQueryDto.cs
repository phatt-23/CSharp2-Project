namespace CoworkingApp.Models.DTOModels.Workspace;

public class WorkspacesQueryDto
{
    public int? Id { get; set; }
    public string? Name { get; set; }
    public int? StatusId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}