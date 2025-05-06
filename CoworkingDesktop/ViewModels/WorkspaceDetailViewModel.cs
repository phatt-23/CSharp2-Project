using CoworkingDesktop.Models;
using CoworkingDesktop.Services;
using CoworkingDesktop.ViewModels.Base;
using CoworkingDesktop.ViewModels.Pagers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CoworkingDesktop.ViewModels
{
    public class WorkspaceDetailViewModel : ViewModelBase
    {
        public string Title => "Workspace Detail";
        public Workspace Workspace { get; private set; }

        public StatusHistoryPagingViewModel StatusHistoryPagingViewModel { get; private set; }
        public WorkspaceReservationsPagerViewModel ReservationsPager { get; private set; }

        private int _totalFinishedReservation;
        public int TotalFinishedReservation { get => _totalFinishedReservation; private set => Set(ref _totalFinishedReservation, value); }

        public WorkspaceDetailViewModel(Workspace workspace, IWorkspacesService workspacesService)
        {
            Workspace = workspace;
            _workspacesService = workspacesService;

            StatusHistoryPagingViewModel = new(Workspace, _workspacesService);
            ReservationsPager = new(Workspace, _workspacesService);

            _ = Init();
        }

        private async Task Init()
        {
            var page = await _workspacesService.GetWorkspaceReservations(Workspace.WorkspaceId, 1, int.MaxValue);

            TotalFinishedReservation = page.Items.Where(res => res.IsCancelled).ToList().Count;
        }

        private IWorkspacesService _workspacesService;
    }
}
