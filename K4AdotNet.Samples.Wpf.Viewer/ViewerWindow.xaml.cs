using System;
using System.Windows;

namespace K4AdotNet.Samples.Wpf.Viewer
{
    internal partial class ViewerWindow : Window
    {
        public ViewerWindow()
            : this(new ViewerModel())
        { }

        public ViewerWindow(ViewerModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
            => (DataContext as ViewerModel)?.Run();

        protected override void OnClosed(EventArgs e)
        {
            (DataContext as ViewerModel)?.Dispose();
            base.OnClosed(e);
        }
    }
}
