using CoworkingDesktop.Models;
using CoworkingDesktop.Services;
using CoworkingDesktop.ViewModels.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoworkingDesktop.ViewModels.Pagers
{
    public class StatusHistoryPagingViewModel : PaginatedViewModel<WorkspaceStatusHistory>
    {
        private readonly IWorkspacesService _workspacesService;
        private readonly Workspace _workspace;

        public StatusHistoryPagingViewModel(Workspace workspace, IWorkspacesService workspacesService)
        {
            _workspacesService = workspacesService;
            _workspace = workspace;
        }

        protected override async Task<PagedResult<WorkspaceStatusHistory>> LoadPageAsync(int page, int pageSize)
        {
            return await _workspacesService.GetStatusHistory(_workspace.WorkspaceId, page, pageSize);
        }
    }
}
