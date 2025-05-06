using CoworkingDesktop.Models;
using CoworkingDesktop.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CoworkingDesktop.ViewModels.Features
{
    public abstract class PaginatedViewModel<T> : ViewModelBase
    {
        private int _currentPage = 1;
        private int _pageSize = 10;
        private int _totalCount;

        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                if (Set(ref _currentPage, value))
                    _ = LoadAsync();
            }
        }

        public int PageSize
        {
            get => _pageSize;
            set
            {
                if (Set(ref _pageSize, value))
                    _ = LoadAsync();
            }
        }

        public int TotalPages => (_totalCount + PageSize - 1) / PageSize;

        public int TotalCount { get => _totalCount; private set => Set(ref _totalCount, value); }

        private ObservableCollection<T> _items = [];
        public ObservableCollection<T> Items
        {
            get => _items;
            set => Set(ref _items, value);
        }

        public ICommand NextPageCommand { get; }
        public ICommand PrevPageCommand { get; }
        public ICommand LoadPageCommand { get; }

        public Func<ICollection<T>, ICollection<T>>? PageResultFilter { get; set; } = null;

        protected PaginatedViewModel()
        {
            NextPageCommand = new RelayCommand(
                () =>
                {
                    if (CurrentPage < TotalPages)
                        CurrentPage++;
                }, 
                () => CurrentPage < TotalPages);

            PrevPageCommand = new RelayCommand(
                () =>
                {
                    if (CurrentPage > 1)
                        CurrentPage--;
                }, 
                () => CurrentPage > 1);

            LoadPageCommand = new RelayCommand(async () => await LoadAsync());
        }

        public async Task LoadAsync()
        {
            var pageResult = await LoadPageAsync(CurrentPage, PageSize);
            TotalCount = pageResult.TotalCount;

            Items = (PageResultFilter == null)
                ? [.. pageResult.Items]
                : [.. PageResultFilter(pageResult.Items)];

            OnPropertyChanged(nameof(TotalPages));
        }

        // This method should be implemented in the derived class to load the data from the API or database.
        protected abstract Task<PagedResult<T>> LoadPageAsync(int page, int pageSize);
    }
}
