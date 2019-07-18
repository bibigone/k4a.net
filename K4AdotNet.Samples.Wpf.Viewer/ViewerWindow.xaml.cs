using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
        {
            (DataContext as ViewerModel)?.Run();
        }

        protected override void OnClosed(EventArgs e)
        {
            (DataContext as ViewerModel)?.Dispose();
            base.OnClosed(e);
        }
    }
}
