using AutoMapper;
using CoworkingDesktop.Helpers;
using CoworkingDesktop.Models;
using CoworkingDesktop.Services;
using CoworkingDesktop.ViewModels.Base;
using CoworkingDesktop.ViewModels.Features;
using CoworkingDesktop.Views.Dialogs;
using System.Windows;
using System.Windows.Input;

namespace CoworkingDesktop.ViewModels
{
    public class ReservationsViewModel : PaginatedCrudViewModel<Reservation>
    {
        private readonly IReservationsService _service;
        private readonly IWorkspacesService _workspacesService;
        private readonly IUsersService _userService;
        private readonly IDialogService _dialog;
        private readonly IMapper _mapper;

        private Reservation? _selected;
        public Reservation? Selected
        {
            get => _selected;
            set => Set(ref _selected, value);
        }

        public ICommand LoadCommand { get; private set; }

        public ReservationsViewModel(
            IReservationsService service, 
            IWorkspacesService workspacesService, 
            IUsersService userService,
            IDialogService dialog, 
            IMapper mapper)
        {
            _service = service;
            _userService = userService;
            _dialog = dialog;
            _mapper = mapper;
            _workspacesService = workspacesService;

            LoadCommand = new RelayCommand(async () => await LoadAsync());
        }

        protected override async Task<PagedResult<Reservation>> LoadPageAsync(int page, int pageSize)
        {
            return await _service.GetReservations(page, pageSize);
        }

        public override async Task Add()
        {
            // show modal dialog, then send via API
            var res = new Reservation();
            var vm = new ReservationFormViewModel(res, FormMode.Add, _workspacesService, _userService);

            vm.RequestClose += async (_, ok) =>
            {
                if (!ok) return;

                var createDto = _mapper.Map<ReservationCreateDto>(res);
                var created = await _service.CreateReservation(createDto);
                if (created == null)
                {
                    MessageBox.Show("Adding reservation failed. Check for collisions.", "Add Reservation Failed", MessageBoxButton.OK);
                    return;
                }

                Items.Add(created);
            };

            _dialog.ShowDialog(vm);
        }

        public override async Task ViewDetail()
        {
            if (Selected == null) return;
            var vm = new ReservationDetailViewModel(Selected);
            _dialog.ShowDialog(vm);
        }

        public override async Task Edit()
        {
            // show modal dialog, then send via API
            if (Selected == null) return;

            var clone = Selected.DeepCopy()!;

            var vm = new ReservationFormViewModel(clone, FormMode.Edit, _workspacesService, _userService);

            vm.RequestClose += async (_, ok) =>
            {
                if (!ok) return;

                var updateDto = _mapper.Map<ReservationUpdateDto>(clone);
                var updated = await _service.UpdateReservation(updateDto);
                if (updated == null)
                {
                    MessageBox.Show("Editing reservation failed. Check for collisions.", "Edit Reservation Failed", MessageBoxButton.OK);
                    return;
                }

                var index = Items.IndexOf(Selected);
                if (index >= 0)
                {
                    Items[index] = updated;
                    Selected = updated;
                }
            };

            _dialog.ShowDialog(vm);
        }

        public override async Task Delete()
        {
            if (Selected == null) return;

            MessageBoxResult result = MessageBox.Show("Do you really want to delete this reservation?", "Reservation Deletion", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
            if (result != MessageBoxResult.Yes)
                return;

            var res = await _service.DeleteReservation(Selected.ReservationId);
            if (res == null) return;

            Items.Remove(Selected);
        }

        public override bool CanEdit()
        {
            return Selected?.StartTime >= DateTime.Now && Selected?.EndTime >= DateTime.Now;
        }

        public override bool CanDelete()
        {
            return CanEdit();
        }

        public override bool CanViewDetail()
        {
            return Selected != null;
        }
    }
}
