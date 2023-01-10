using System;
using System.Windows.Input;

namespace K4AdotNet.Samples.Wpf
{
    public class DelegateCommand : ICommand
    {
        private readonly Action execute;
        private readonly Func<bool>? canExecute;

        public DelegateCommand(Action execute, Func<bool>? canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged;

        public void InvalidateCanExecute() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

        public bool CanExecute(object? parameter) => canExecute?.Invoke() != false;

        public void Execute(object? parameter) => execute?.Invoke();
    }
}
