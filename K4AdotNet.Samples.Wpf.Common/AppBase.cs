using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;

namespace K4AdotNet.Samples.Wpf
{
    public abstract class AppBase : Application, IApp
    {
        protected AppBase()
            => SetDefaultCulture();

        protected static void SetDefaultCulture()
        {
            // Set default culture to en-US because UI is in English
            // and it is also a default culture for formatting values in bindings.
            // https://social.msdn.microsoft.com/Forums/vstudio/en-US/dd94f1f8-4213-49c3-903f-780b901a30d0/data-binding-ignores-local-culture-settings?forum=wpf
            var defaultCulture = CultureInfo.GetCultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture = defaultCulture;
            CultureInfo.DefaultThreadCurrentCulture = defaultCulture;
        }

        protected abstract Window? CreateMainWindow(StartupEventArgs e);

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

        public void ShowErrorMessage(string message, string? title = null)
            => MessageBox.Show(CurrentWindow, message, title ?? "Error", MessageBoxButton.OK, MessageBoxImage.Error);

        public abstract void ShowWindowForModel(object viewModel);

        public string? BrowseFileToOpen(string filter, string? title = null)
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
