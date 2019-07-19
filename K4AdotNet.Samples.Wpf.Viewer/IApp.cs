using System;
using System.Windows.Threading;

namespace K4AdotNet.Samples.Wpf.Viewer
{
    internal interface IApp
    {
        Dispatcher Dispatcher { get; }
        void ShowErrorMessage(string message, string title = null);
        void ShowWindowForModel(object viewModel);
        string BrowseFileToOpen(string filter, string title = null);
        IDisposable IndicateWaiting();
    }
}
