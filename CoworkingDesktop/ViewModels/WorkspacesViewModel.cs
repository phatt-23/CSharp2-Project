using AutoMapper;
using CoworkingDesktop.Helpers;
using CoworkingDesktop.Models;
using CoworkingDesktop.Services;
using CoworkingDesktop.ViewModels.Base;
using CoworkingDesktop.ViewModels.Features;
using CoworkingDesktop.Views.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace CoworkingDesktop.ViewModels
{
    public class WorkspacesViewModel : PaginatedCrudViewModel<Workspace>
    {
        private readonly IWorkspacesService _workspacesService;
        private readonly ICoworkingCenterService _centersService;
        private readonly IDialogService _dialog;
        private readonly IMapper _mapper;

        public WorkspacesViewModel(
            IWorkspacesService workspacesService, 
            ICoworkingCenterService centersService,
            IDialogService dialog, 
            IMapper mapper)
        {
            _workspacesService = workspacesService;
            _dialog = dialog;
            _mapper = mapper;
            _centersService = centersService;
        }

        public ICommand LoadCommand { get; private set; }

        protected override async Task<PagedResult<Workspace>> LoadPageAsync(int page, int pageSize)
        {
            return await _workspacesService.GetWorkspaces(page, pageSize);
        }

        public override async Task Add()
        {
            // show modal dialog, then send via API
            var ws = new Workspace();
            var vm = new WorkspaceFormViewModel(ws, FormMode.Add, _centersService);

            vm.RequestClose += async (_, ok) =>
            {
                if (!ok) return;

                var createDto = _mapper.Map<WorkspaceCreateDto>(ws);
                var created = await _workspacesService.CreateWorkspace(createDto);
                if (created == null)
                {
                    MessageBox.Show("Adding workspace failed.", "Add Workspace Failed", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                Items.Add(created);
            };

            _dialog.ShowDialog(vm);
        }

        public override async Task Delete()
        {
            if (SelectedItem == null) return;

            MessageBoxResult result = MessageBox.Show($"Do you really want to delete this '{SelectedItem.Name}' workspace?", "Delete Workspace", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
            if (result != MessageBoxResult.Yes)
                return;

            var res = await _workspacesService.DeleteWorkspace(SelectedItem.WorkspaceId);
            if (res == null) return;

            Items.Remove(SelectedItem);
            SelectedItem = null;
        }

        public override async Task Edit()
        {
            if (SelectedItem == null) return;

            // show modal dialog, then send via API
            var clone = this.SelectedItem.DeepCopy()!;
            var vm = new WorkspaceFormViewModel(clone, FormMode.Edit, _centersService);

            vm.RequestClose += async (_, ok) =>
            {
                if (!ok) return;

                var updateDto = _mapper.Map<WorkspaceUpdateDto>(clone);
                var updated = await _workspacesService.UpdateWorkspace(updateDto);
                if (updated == null)
                {
                    MessageBox.Show("Editing workspace failed.", "Edit Workspace Failed", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                int index = Items.IndexOf(SelectedItem);
                if (index >= 0)
                {
                    Items[index] = updated;
                    SelectedItem = updated;
                }
            };

            _dialog.ShowDialog(vm);
        }

        public override async Task ViewDetail()
        {
            if (SelectedItem == null) return;
            var vm = new WorkspaceDetailViewModel(SelectedItem, _workspacesService);
            _dialog.ShowDialog(vm);
        }

        public override bool CanEdit() => SelectedItem != null;
        public override bool CanDelete() => SelectedItem != null;
        public override bool CanViewDetail() => SelectedItem != null;
    }
}
