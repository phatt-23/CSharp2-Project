using CoworkingDesktop.ViewModels.Features;
using CoworkingDesktop.Views.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CoworkingDesktop.ViewModels.Base
{
    public abstract class ClosableFormViewModel : ViewModelBase, IClosableDialog
    {
        public abstract bool CanSave();
        public string? ErrorMessage { get => _errorMessage; set => Set(ref _errorMessage, value); }

        public FormMode Mode { get => _mode; private set => Set(ref _mode, value); }

        public bool IsInAddMode => Mode == FormMode.Add;
        public bool IsInEditMode => Mode == FormMode.Edit;

        public ICommand SaveCommand { get; }
        public ICommand CloseCommand { get; set; }

        protected ClosableFormViewModel(FormMode mode)
        {
            Mode = mode;
            CloseCommand = new RelayCommand(() => RequestClose?.Invoke(this, false));
            SaveCommand = new RelayCommand(() => RequestClose?.Invoke(this, true), CanSave);
        }

        private FormMode _mode;
        private string? _errorMessage;
        public event EventHandler<bool>? RequestClose;
    }
}
