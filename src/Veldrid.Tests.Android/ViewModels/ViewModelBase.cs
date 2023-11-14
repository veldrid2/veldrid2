using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Veldrid.Tests.Android.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool Set<T>(ref T destination, T value, [CallerMemberName] string? propertyName = null)
        {
            if (!EqualityComparer<T>.Default.Equals(destination, value))
            {
                destination = value;

                RaisePropertyChanged(propertyName);
                return true;
            }
            return false;
        }
    }
}
