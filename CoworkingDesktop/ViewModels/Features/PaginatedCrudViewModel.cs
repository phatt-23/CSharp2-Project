using CoworkingDesktop.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CoworkingDesktop.ViewModels.Features
{
    public abstract class PaginatedCrudViewModel<T> : PaginatedViewModel<T>, ICrudDataGrid
    {
        private T? _selectedItem;
        public T? SelectedItem
        {
            get => _selectedItem;
            set => Set(ref _selectedItem, value);
        }

        public ICommand AddCommand { get; protected set; }
        public ICommand ViewDetailCommand { get; protected set; }
        public ICommand EditCommand { get; protected set; }
        public ICommand DeleteCommand { get; protected set; }

        protected PaginatedCrudViewModel()
        {
            AddCommand = new RelayCommand(async () => await Add());
            ViewDetailCommand = new RelayCommand(async () => await ViewDetail(), CanViewDetail);
            EditCommand = new RelayCommand(async () => await Edit(), CanEdit);
            DeleteCommand = new RelayCommand(async () => await Delete(), CanDelete);
        }

        public abstract Task Add();
        public abstract Task Delete();
        public abstract Task Edit();
        public abstract Task ViewDetail();

        public virtual bool CanDelete() => SelectedItem != null;
        public virtual bool CanEdit() => SelectedItem != null;
        public virtual bool CanViewDetail() => SelectedItem != null;
    }
}
