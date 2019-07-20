using System;
using System.Windows;

namespace K4AdotNet.Samples.Wpf.BodyTracker
{
    internal sealed class App : AppBase
    {
        [STAThread]
        public static void Main()
            => new App().Run();

        protected override Window CreateMainWindow(StartupEventArgs e)
        {
            if (!Sdk.CheckPrerequisitesForBodyTracking(out var message))
            {
                MessageBox.Show(message, "Incompatible Environment");
                return null;
            }

            return new MainWindow(new MainModel(this));
        }

        public override void ShowWindowForModel(object viewModel)
        {
            if (viewModel is TrackerModel trackerModel)
                new TrackerWindow(trackerModel) { Owner = CurrentWindow }.Show();
            else
                throw new NotSupportedException();
        }
    }
}
