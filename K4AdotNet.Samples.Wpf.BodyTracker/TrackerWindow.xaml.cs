using System;
using System.Windows;

namespace K4AdotNet.Samples.Wpf.BodyTracker
{
    partial class TrackerWindow : Window
    {
        public TrackerWindow()
            : this(new TrackerModel())
        { }

        public TrackerWindow(TrackerModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            (DataContext as TrackerModel)?.Run();
        }

        protected override void OnClosed(EventArgs e)
        {
            (DataContext as TrackerModel)?.Dispose();
            base.OnClosed(e);
        }
    }
}
