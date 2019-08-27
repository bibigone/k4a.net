using System;
using System.Windows;

namespace K4AdotNet.Samples.Wpf.BackgroundRemover
{
    internal class App : AppBase
    {
        [STAThread]
        public static void Main()
            => new App().Run();

        protected override Window CreateMainWindow(StartupEventArgs e)
        {
            var startModel = new StartViewModel(this);
            return new StartWindow
            {
                DataContext = startModel
            };
        }

        public override void ShowWindowForModel(object viewModel)
        {
            Window window;
            if (viewModel is StartViewModel)
                window = new StartWindow();
            else if (viewModel is ProcessingViewModel)
                window = new ProcessingWindow();
            else
                throw new ArgumentOutOfRangeException(nameof(viewModel));

            window.DataContext = viewModel;
            window.Owner = CurrentWindow;
            window.Show();

            window.Closed += (_, __) => (viewModel as IDisposable)?.Dispose();
        }
    }
}
