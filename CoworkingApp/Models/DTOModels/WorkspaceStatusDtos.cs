namespace CoworkingApp.Models.DtoModels;

[PublicDataDto]
public class WorkspaceStatusDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
}

[PublicRequestDto]
public class WorkspaceStatusQueryRequestDto
{
    public int? Id { get; set; }
    public string? Name { get; set; }
}

[PublicResponseDto]
public class WorkspaceStatusesResponseDto
{
    public required int TotalCount { get; set; }
    public required IEnumerable<WorkspaceStatusDto> WorkspaceStatuses { get; set; }
}
