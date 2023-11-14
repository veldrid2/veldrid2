using System;
using System.Diagnostics;
using System.Windows.Input;

namespace Veldrid.Tests.Android.Utilities
{
    class DelegateCommand : ICommand
    {
        readonly Func<bool>? canExecute;
        readonly Action execute;

        public DelegateCommand(Action execute, Func<bool>? canExecute = null)
        {
            this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
            this.canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged;

        [DebuggerStepThrough]
        public bool CanExecute(object? p)
        {
            if (canExecute == null)
            {
                return true;
            }
            try
            {
                return canExecute();
            }
            catch
            {
                return false;
            }
        }

        public void Execute(object? p)
        {
            if (!CanExecute(p))
            {
                return;
            }
            execute();
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    class DelegateCommand<T> : ICommand
    {
        readonly Func<T, bool>? canExecute;
        readonly Action<T> execute;

        public DelegateCommand(Action<T> execute, Func<T, bool>? canExecute = null)
        {
            this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
            this.canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged;

        [DebuggerStepThrough]
        public bool CanExecute(object? p)
        {
            if (canExecute == null)
            {
                return true;
            }
            try
            {
                if (p != null && p is not T)
                {
                    p = (T)Convert.ChangeType(p, typeof(T));
                }
                return canExecute.Invoke((T)p!);
            }
            catch
            {
                return false;
            }
        }

        public void Execute(object? p)
        {
            if (!CanExecute(p))
            {
                return;
            }
            if (p != null && p is not T)
            {
                p = (T)Convert.ChangeType(p, typeof(T));
            }
            execute((T)p!);
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
