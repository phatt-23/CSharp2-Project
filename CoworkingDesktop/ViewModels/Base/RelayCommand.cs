using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CoworkingDesktop.ViewModels.Base
{
    public class RelayCommand<T>(Action<T?> execute, Func<T?, bool>? canExecute = null) : ICommand
    {
        private readonly Action<T?> _execute = execute;
        private readonly Func<T?, bool>? _canExecute = canExecute;

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object? parameter) => _canExecute?.Invoke((T?)parameter) ?? true;

        public void Execute(object? parameter) => _execute((T?)parameter);

        public void RaiseCanExecuteChanged() => CommandManager.InvalidateRequerySuggested();
    }

    public class RelayCommand : RelayCommand<object>
    {
        public RelayCommand(Action execute) : base(_ => execute()) 
        { 
        }

        public RelayCommand(Action execute, Func<bool> canExecute) : base(_ => execute(), _ => canExecute()) 
        { 
        }
    }
}
