using System;
using System.Linq;
using System.Windows;

namespace K4AdotNet.Samples.Wpf
{
    public abstract class AppBase : Application, IApp
    {
        protected abstract Window CreateMainWindow(StartupEventArgs e);

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ShutdownMode = ShutdownMode.OnExplicitShutdown;
            var mainWindow = CreateMainWindow(e);
            if (mainWindow == null)
            {
                Shutdown();
            }
            else
            {
                ShutdownMode = ShutdownMode.OnMainWindowClose;
                MainWindow = mainWindow;
                mainWindow.Show();
            }
        }

        public IDisposable IndicateWaiting()
            => new MouseCursorOverrider(System.Windows.Input.Cursors.Wait);

        public void ShowErrorMessage(string message, string title = null)
            => MessageBox.Show(CurrentWindow, message, title ?? "Error", MessageBoxButton.OK, MessageBoxImage.Error);

        public abstract void ShowWindowForModel(object viewModel);

        public string BrowseFileToOpen(string filter, string title = null)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                CheckFileExists = true,
                CheckPathExists = true,
                Multiselect = false,
                Filter = filter,
                Title = title ?? "Choose file to open",
            };

            if (openFileDialog.ShowDialog(CurrentWindow) != true)
                return null;

            return openFileDialog.FileName;
        }

        public Window CurrentWindow
            => Windows.OfType<Window>().FirstOrDefault(w => w.IsActive) ?? MainWindow;

        private sealed class MouseCursorOverrider : IDisposable
        {
            private readonly System.Windows.Input.Cursor oldValue;

            public MouseCursorOverrider(System.Windows.Input.Cursor cursorToBeShown)
            {
                oldValue = System.Windows.Input.Mouse.OverrideCursor;
                System.Windows.Input.Mouse.OverrideCursor = cursorToBeShown;
            }

            public void Dispose()
                => System.Windows.Input.Mouse.OverrideCursor = oldValue;
        }
    }
}
