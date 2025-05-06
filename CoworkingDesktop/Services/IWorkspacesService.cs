using CoworkingDesktop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoworkingDesktop.Services
{
    public interface IWorkspacesService
    {
        Task<PagedResult<Workspace>> GetWorkspaces(int page, int pageSize);
        Task<Workspace?> GetWorkspaceById(int id);
        Task<Workspace?> CreateWorkspace(WorkspaceCreateDto dto);
        Task<Workspace?> UpdateWorkspace(WorkspaceUpdateDto dto);
        Task<Workspace?> DeleteWorkspace(int id);
        Task<PagedResult<WorkspaceStatusHistory>> GetStatusHistory(int id, int page, int pageSize);
        Task<PagedResult<Reservation>> GetWorkspaceReservations(int id, int page, int pageSize);
    }
}
