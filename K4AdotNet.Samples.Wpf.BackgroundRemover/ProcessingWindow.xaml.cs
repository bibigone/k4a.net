using System;
using System.Windows;

namespace K4AdotNet.Samples.Wpf.BackgroundRemover
{
    public partial class ProcessingWindow : Window
    {
        public ProcessingWindow()
        {
            InitializeComponent();

            Loaded += ProcessingWindow_Loaded;
        }

        private ProcessingViewModel ViewModel => (ProcessingViewModel)DataContext;

        private void ProcessingWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel?.Run();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            ViewModel?.Dispose();
        }
    }
}
