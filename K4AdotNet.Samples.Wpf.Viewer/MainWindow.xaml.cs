using K4AdotNet.Sensor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;

namespace K4AdotNet.Samples.Wpf.Viewer
{
    internal partial class MainWindow : Window
    {
        public MainWindow()
        {
            DataContext = this;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var readLoop = BackgroundReadLoop.CreateForPlayback(@"D:\tmp\NFOV_UNBINNED.mkv", disableColor: true, disableDepth: false, playAsFastAsPossible: true);
            new ViewerWindow(new ViewerModel(Dispatcher, readLoop)).Show();
        }

        private void OpenPlayback(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = ".mkv",
                Multiselect = false,
                Filter = "MKV recordings|*.mkv",
                Title = "Choose recording for playback",
            };

            if (openFileDialog.ShowDialog(this) == true)
            {
                try
                {
                    System.Windows.Input.Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                    try
                    {
                        var readLoop = BackgroundReadLoop.CreateForPlayback(openFileDialog.FileName,
                            disableColor: checkBoxPlaybackDisableColor.IsChecked == true,
                            disableDepth: checkBoxPlaybackDisableDepth.IsChecked == true,
                            playAsFastAsPossible: checkBoxPlaybackFast.IsChecked == true);
                        new ViewerWindow(new ViewerModel(Dispatcher, readLoop)) { Owner = this }.Show();
                    }
                    finally
                    {
                        System.Windows.Input.Mouse.OverrideCursor = null;
                    }
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message, "Cannot open file for playback", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
