using System;
using System.Linq;
using System.Windows;

namespace K4AdotNet.Samples.Wpf.Viewer
{
    partial class App : Application, IApp
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            MainWindow = new MainWindow(new MainModel(this));
            MainWindow.Show();
        }

        public IDisposable IndicateWaiting()
            => new MouseCursorOverrider(System.Windows.Input.Cursors.Wait);

        public void ShowErrorMessage(string message, string title = null)
            => MessageBox.Show(CurrentWindow, message, title ?? "Error", MessageBoxButton.OK, MessageBoxImage.Error);

        public void ShowWindowForModel(object viewModel)
        {
            if (viewModel is ViewerModel viewerModel)
                new ViewerWindow(viewerModel) { Owner = CurrentWindow }.Show();
            else
                throw new NotSupportedException();
        }

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

        private Window CurrentWindow
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
