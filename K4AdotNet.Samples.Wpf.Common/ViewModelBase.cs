using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Threading;

namespace K4AdotNet.Samples.Wpf
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        protected readonly IApp? app;
        protected readonly Dispatcher dispatcher;

        protected ViewModelBase()
            => dispatcher = Dispatcher.CurrentDispatcher;

        protected ViewModelBase(IApp app)
        {
            this.app = app ?? throw new ArgumentNullException(nameof(app));
            dispatcher = app.Dispatcher;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            if (Thread.CurrentThread != dispatcher.Thread)
            {
                // If this method is called from non-UI thread
                // then redirect it to UI thread
                // in async manner to protect us from deadlocks
                dispatcher.BeginInvoke(DispatcherPriority.DataBind, new Action(() => RaisePropertyChanged(propertyName)));
                return;
            }

            OnPropertyChanged(propertyName);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Invoked just before <see cref="PropertyChanged"/> event is raised.
        /// </summary>
        /// <remarks>
        /// Default implementation does nothing.
        /// The method is invoked on <see cref="dispatcher"/>'s thread.
        /// </remarks>
        protected virtual void OnPropertyChanged(string propertyName)
        {
        }

        protected bool SetPropertyValue<T>(ref T field, T value, [CallerMemberName] string? propertyName = null, string? dependentPropertyName = null)
        {
            if (!Equals(field, value))
            {
                field = value;

                RaisePropertyChanged(propertyName!);

                if (!string.IsNullOrEmpty(dependentPropertyName))
                    RaisePropertyChanged(dependentPropertyName);

                return true;
            }

            return false;
        }
    }
}
