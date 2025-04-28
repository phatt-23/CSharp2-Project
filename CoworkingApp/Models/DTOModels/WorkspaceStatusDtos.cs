namespace CoworkingApp.Models.DtoModels;

/// Data DTO
public class WorkspaceStatusDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
}

/// Request DTO
public class WorkspaceStatusQueryRequestDto
{
    public int? Id { get; set; }
    public string? Name { get; set; }
}

/// Response DTO
public class WorkspaceStatusesResponseDto
{
    public required int TotalCount { get; set; }
    public required IEnumerable<WorkspaceStatusDto> WorkspaceStatuses { get; set; }
}
