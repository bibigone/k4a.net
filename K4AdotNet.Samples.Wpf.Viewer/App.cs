using System;
using System.Windows;

namespace K4AdotNet.Samples.Wpf.Viewer
{
    internal sealed class App : AppBase
    {
        [STAThread]
        public static void Main()
        {
            try
            {
#if DEBUG
                Sdk.TraceLevel = System.Diagnostics.TraceLevel.Info;
#else
                Sdk.TraceLevel = System.Diagnostics.TraceLevel.Warning;
#endif

                new App().Run();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR");
            }
        }

        protected override Window CreateMainWindow(StartupEventArgs e)
            => new MainWindow(new(this));

        public override void ShowWindowForModel(object viewModel)
        {
            if (viewModel is ViewerModel viewerModel)
                new ViewerWindow(viewerModel) { Owner = CurrentWindow }.Show();
            else
                throw new NotSupportedException();
        }
    }
}
