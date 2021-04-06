using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;

namespace K4AdotNet.Samples.Wpf.BodyTracker
{
    partial class BodyTrackingInitializationDialog : Window
    {
        private bool isClosed;

        public BodyTrackingInitializationDialog()
        {
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            bool isOk = false;
            string message = null;
            await Task.Run(() => isOk = Sdk.TryInitializeBodyTrackingRuntime(BodyTracking.TrackerProcessingMode.Cpu, out message))
                .ConfigureAwait(true);

            if (isClosed)
                return;

            if (isOk)
            {
                DialogResult = true;
                Close();
            }

            progressBar.Visibility = Visibility.Hidden;
            waitTextBlock.Visibility = Visibility.Collapsed;
            errorTextBlock.Visibility = Visibility.Visible;
            messageRun.Text = message;
            closeButton.Content = "Close";
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            isClosed = true;
        }

        private void SetupHyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(Sdk.BodyTrackingSdkInstallationGuideUrl));
            e.Handled = true;
        }
    }
}
