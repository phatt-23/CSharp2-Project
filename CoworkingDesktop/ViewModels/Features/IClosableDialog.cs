using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CoworkingDesktop.ViewModels.Features
{
    public interface IClosableDialog
    {
        event EventHandler<bool>? RequestClose;
        ICommand CloseCommand { get; }
    }
}
