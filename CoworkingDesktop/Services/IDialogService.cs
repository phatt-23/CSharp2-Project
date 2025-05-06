using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoworkingDesktop.Services
{
    public interface IDialogService
    {
        bool? ShowDialog(object viewModel);
    }
}
