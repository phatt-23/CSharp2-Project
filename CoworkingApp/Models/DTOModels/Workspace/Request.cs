using System.ComponentModel.DataAnnotations;
using CoworkingApp.Models.DataModels;

namespace CoworkingApp.Models.DTOModels.Workspace;

//////////////////////////////////////////////////////////////
// DTO representing a request (that doesn't hold any data)
//////////////////////////////////////////////////////////////

public class WorkspaceQueryRequestDto : PaginationRequestDto
{
    public string? Name { get; set; }
}


[AdminDto]
public class AdminWorkspaceQueryRequestDto : WorkspaceQueryRequestDto
{
    // Only admin can filter by status, because users only see available ones
    public int? StatusId { get; set; }
    public WorkspaceStatusType? StatusType { get; set; }
}


[AdminDto]
public class WorkspaceCreateRequestDto
{
    [Required]
    public string Name { get; set; } = null!;

    [Required]
    public string Description { get; set; } = null!;

    [Required]
    public int CoworkingCenterId { get; set; }

    [Required]
    public int StatusId { get; set; }
}

