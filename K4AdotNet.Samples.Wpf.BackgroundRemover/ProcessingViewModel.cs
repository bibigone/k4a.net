using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K4AdotNet.Samples.Wpf.BackgroundRemover
{
    internal class ProcessingViewModel : ViewModelBase, IDisposable
    {
        private readonly BackgroundReadingLoop readingLoop;

        public ProcessingViewModel(BackgroundReadingLoop readingLoop)
        {
            this.readingLoop = readingLoop;
        }

        public void Dispose()
        {
            readingLoop.Dispose();
        }
    }
}
