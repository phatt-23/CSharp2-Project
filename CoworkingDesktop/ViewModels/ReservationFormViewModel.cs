using CoworkingDesktop.Models;
using CoworkingDesktop.Services;
using CoworkingDesktop.ViewModels.Base;
using CoworkingDesktop.ViewModels.Features;
using CoworkingDesktop.ViewModels.Pagers;
using CoworkingDesktop.Views.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace CoworkingDesktop.ViewModels
{
    public class ReservationFormViewModel : ViewModelBase, IClosableDialog
    {
        private readonly IWorkspacesService _workspaceService;
        private readonly IUsersService _userService;

        private Reservation _reservation;
        public Reservation Reservation { get => _reservation; private set => Set(ref _reservation, value); }

        private ObservableCollection<User> _users = [];
        public ObservableCollection<User> Users { get => _users; private set => Set(ref _users, value); }

        private ObservableCollection<Workspace> _workspaces = [];
        public ObservableCollection<Workspace> Workspaces { get => _workspaces; private set => Set(ref _workspaces, value); }

        private string _title;
        public string Title { get => _title; private set => Set(ref _title, value); }

        public DateTime? _startDate, _endDate;
        public DateTime? StartDate { get => _startDate; set => Set(ref _startDate, value); }
        public DateTime? EndDate { get => _endDate; set => Set(ref _endDate, value); }

        public string? _startTime, _endTime;
        public string? StartTime { get => _startTime; set => Set(ref _startTime, value); }
        public string? EndTime { get => _endTime; set => Set(ref _endTime, value); }

        private FormMode _mode;
        public FormMode Mode { get => _mode; private set => Set(ref _mode, value); }

        public bool IsInEditMode => Mode == FormMode.Edit;
        public bool IsInAddMode => Mode == FormMode.Add;

        private string? _errorMessage = null;
        public string? ErrorMessage { get => _errorMessage; set => Set(ref _errorMessage, value); }

        private User? _selectedUser;
        public User? SelectedUser { get => _selectedUser; set => Set(ref _selectedUser, value); }

        private Workspace? _selectedWorkspace;
        public Workspace? SelectedWorkspace { get => _selectedWorkspace; set => Set(ref _selectedWorkspace, value); }

        private decimal _totalPrice;
        public decimal TotalPrice { get => _totalPrice; private set => Set(ref _totalPrice, value); }

        public UserPagingViewModel UserPagingViewModel { get; private set; }
        public WorkspacePagingViewModel WorkspacePagingViewModel { get; private set; }

        public ReservationFormViewModel(Reservation reservation, FormMode mode, IWorkspacesService workspaceService, IUsersService userService)
        {
            _title = Mode.ToString() + " Reservation";
            Mode = mode;
            _reservation = reservation;
            _workspaceService = workspaceService;
            _userService = userService;

            UserPagingViewModel = new(_userService) { PageResultFilter = (users) => [.. users.Where(w => !w.IsRemoved)] };

            WorkspacePagingViewModel = new(_workspaceService) {
                PageResultFilter = (workspaces) => [.. workspaces.Where(w => w.Status == WorkspaceStatusType.Available)]
            };

            SaveCommand = new RelayCommand(() => RequestClose?.Invoke(this, true), CanSave);
            CloseCommand = new RelayCommand(() => RequestClose?.Invoke(this, false));

            _ = Init();
        }

        private async Task Init()
        {
            if (Mode == FormMode.Add)
            {
                Reservation.StartTime = DateTime.Now.AddHours(1);
                Reservation.EndTime = DateTime.Now.AddDays(1);
            }
            else if (Mode == FormMode.Edit)
            {
                SelectedUser = await _userService.GetUserById(Reservation.CustomerId);
                SelectedWorkspace = await _workspaceService.GetWorkspaceById(Reservation.WorkspaceId);
            }

            StartDate = Reservation.StartTime.Date;
            EndDate = Reservation.EndTime.Date;
            StartTime = Reservation.StartTime.TimeOfDay.ToString(@"hh\:mm");
            EndTime = Reservation.EndTime.TimeOfDay.ToString(@"hh\:mm");
        }

        private bool CanSave()
        {
            ErrorMessage = null;

            if (/* IsInAddMode && */ SelectedUser == null)
            {
                ErrorMessage = "Must select a user";
                return false;
            }

            if (SelectedWorkspace == null)
            {
                ErrorMessage = "Must select a workspace";
                return false;
            }

            if (string.IsNullOrWhiteSpace(StartTime) || string.IsNullOrWhiteSpace(EndTime))
            {
                ErrorMessage = "Time is empty";
                return false;
            }

            if (!DateTime.TryParse(StartTime, out DateTime start) || !DateTime.TryParse(EndTime, out DateTime end))
            {
                ErrorMessage = "Time is in wrong format";
                return false;
            }

            if (!DateTime.TryParse($"{StartDate?.ToShortDateString()} {StartTime}", out var startDateTime) ||
                !DateTime.TryParse($"{EndDate?.ToShortDateString()} {EndTime}", out var endDateTime))
            {
                ErrorMessage = "Couldn't parse time";
                return false;
            }

            if (startDateTime >= endDateTime)
            {
                ErrorMessage = "Start datetime must be before end datetime";
                return false;
            }

            Reservation.StartTime = startDateTime;
            Reservation.EndTime = endDateTime;
            Reservation.WorkspaceId = SelectedWorkspace.WorkspaceId;
            if (IsInAddMode)
            {
                Reservation.CustomerId = SelectedUser.UserId;
            }

            TotalPrice = SelectedWorkspace.PricePerHour * (decimal)(Reservation.EndTime - Reservation.StartTime).TotalHours;

            return true;
        }

        public ICommand SaveCommand { get; }
        public ICommand CloseCommand { get; } 

        public event EventHandler<bool>? RequestClose;

    }
}
