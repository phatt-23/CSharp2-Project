using CoworkingDesktop.Models;
using CoworkingDesktop.Services;
using CoworkingDesktop.ViewModels.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CoworkingDesktop.ViewModels.Pagers
{
    public class WorkspacePagingViewModel : PaginatedViewModel<Workspace>
    {
        public WorkspacePagingViewModel(IWorkspacesService workspaceService)
        {
            _workspaceService = workspaceService;
        }

        private readonly IWorkspacesService _workspaceService;

        protected override async Task<PagedResult<Workspace>> LoadPageAsync(int page, int pageSize)
        {
            return await _workspaceService.GetWorkspaces(page, pageSize);
        }
    }
}
