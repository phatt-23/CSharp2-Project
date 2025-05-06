using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CoworkingDesktop.ViewModels.Features
{
    public interface ICrudDataGrid
    {
        //ICommand LoadCommand { get; }
        ICommand AddCommand { get; }
        ICommand ViewDetailCommand { get; }
        ICommand EditCommand { get; }
        ICommand DeleteCommand { get; }

        //Task Load();
        Task Add();
        Task Edit();
        Task Delete();
        Task ViewDetail();

        bool CanDelete();
        bool CanEdit();
        bool CanViewDetail();
    }
}
