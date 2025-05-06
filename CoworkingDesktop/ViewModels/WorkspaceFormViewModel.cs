using CoworkingDesktop.Models;
using CoworkingDesktop.Services;
using CoworkingDesktop.ViewModels.Base;
using CoworkingDesktop.ViewModels.Features;
using CoworkingDesktop.ViewModels.Pagers;
using CoworkingDesktop.Views.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CoworkingDesktop.ViewModels
{
    public class WorkspaceFormViewModel : ViewModelBase, IClosableDialog
    {
        private readonly ICoworkingCenterService _centerService;

        private Workspace _workspace;
        public Workspace Workspace { get => _workspace; private set => Set(ref _workspace, value); }

        private ObservableCollection<WorkspaceStatusType> _statuses = [];
        public ObservableCollection<WorkspaceStatusType> Statuses { get => _statuses; private set => Set(ref _statuses, value); }

        private CoworkingCenter? _selectedCenter;
        public CoworkingCenter? SelectedCenter
        {
            get => _selectedCenter;
            set
            {
                Set(ref _selectedCenter, value);
                if (value != null) 
                    Workspace.CoworkingCenterId = value.CoworkingCenterId;
            }
        }

        private WorkspaceStatusType? _selectedStatus;
        public WorkspaceStatusType? SelectedStatus
        {
            get => _selectedStatus;
            set
            {
                Set(ref _selectedStatus, value);
                if (value != null)
                    Workspace.Status = value.Value;
            }
        }

        private FormMode _mode;
        public FormMode Mode { get => _mode; private set => Set(ref _mode, value); }

        private string _title;
        public string Title { get => _title; private set => Set(ref _title, value); }

        public bool IsInEditMode => Mode == FormMode.Edit;
        public bool IsInAddMode => Mode == FormMode.Add;

        private string? _errorMessage;
        public string? ErrorMessage { get => _errorMessage; private set => Set(ref _errorMessage, value); }

        public CoworkingCenterPagingViewModel CenterPagingViewModel { get; }
        
        public WorkspaceFormViewModel(Workspace workspace, FormMode mode, ICoworkingCenterService centerService)
        {
            Workspace = workspace;
            Mode = mode;
            Title = Mode.ToString() + " Workspace";
            _centerService = centerService;

            CenterPagingViewModel = new(_centerService);

            Statuses = [.. Enum.GetNames(typeof(WorkspaceStatusType)).Select(n => Enum.Parse<WorkspaceStatusType>(n))];

            SaveCommand = new RelayCommand(() => RequestClose?.Invoke(this, true), CanSave);
            CloseCommand = new RelayCommand(() => RequestClose?.Invoke(this, false));

            _ = Init();
        }

        private async Task Init()
        {
            if (IsInEditMode)
            {
                var center = await _centerService.GetCoworkingCenterById(Workspace.CoworkingCenterId);
                if (center == null)
                {
                    ErrorMessage = "Coworking center of this workspace not found.";
                    return;
                }

                CenterPagingViewModel.Items.Add(center);
                SelectedCenter = center;

                SelectedStatus = Workspace.Status;
            }
        }

        public event EventHandler<bool>? RequestClose;
        public ICommand SaveCommand { get; }
        public ICommand CloseCommand { get; }

        private bool CanSave()
        {
            ErrorMessage = null;

            if (string.IsNullOrEmpty(Workspace.Name))
            {
                ErrorMessage = "Workspace name cannot be empty";
                return false;
            }
            if (SelectedCenter == null)
            {
                ErrorMessage = "Coworking center cannot be empty";
                return false;
            }
            if (SelectedStatus == null)
            {
                ErrorMessage = "Workspace status cannot be empty";
                return false;
            }
            if (Workspace.PricePerHour < 0)
            {
                ErrorMessage = "Workspace price cannot be negative";
                return false;
            }

            return true;
        }
    }
}
