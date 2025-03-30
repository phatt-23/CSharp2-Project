using CoworkingApp.Models.DTOModels.CoworkingCenters;
using CoworkingApp.Models.DTOModels.WorkspaceStatus;

namespace CoworkingApp.Models.DTOModels.Workspace;

public class WorkspaceDetailDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public CoworkingCenterDto CoworkingCenter { get; set; } = null!;

    public WorkspaceStatusDto Status { get; set; } = null!;
}