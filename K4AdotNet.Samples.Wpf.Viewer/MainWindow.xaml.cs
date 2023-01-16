using System.Windows;

namespace K4AdotNet.Samples.Wpf.Viewer
{
    internal partial class MainWindow : Window
    {
        public MainWindow()
            => InitializeComponent();

        public MainWindow(MainModel viewModel)
            : this()
            => DataContext = viewModel;

        private void Window_Loaded(object sender, RoutedEventArgs e)
            => ViewModel?.RefreshDevices();

        private MainModel? ViewModel => DataContext as MainModel;

        private void OpenPlayback(object sender, RoutedEventArgs e)
            => ViewModel!.Playback();

        private void OpenDevice(object sender, RoutedEventArgs e)
            => ViewModel!.OpenDevice();

        private void RefreshDevices(object sender, RoutedEventArgs e)
            => ViewModel!.RefreshDevices();
    }
}
