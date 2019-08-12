using System;
using System.IO;
using System.Threading;

namespace K4AdotNet
{
    // Temporary overrides current directory
    // Usage:
    // using (new CurrentDirectoryOverrider(someDirectoryRequiredToBeCurrent))
    // {
    //      // some logic which requires specific current directory
    // }
    internal sealed class CurrentDirectoryOverrider : IDisposable
    {
        private static readonly object syncRoot = new object();         // to avoid concurrent overriding from different threads

        private readonly string path;
        private readonly string currentDirBak;

        public CurrentDirectoryOverrider(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            this.path = Path.GetFullPath(path);
            Monitor.Enter(syncRoot);
            currentDirBak = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(this.path);
        }

        public void Dispose()
        {
            var currentDir = Directory.GetCurrentDirectory();
            if (!path.Equals(currentDir, StringComparison.InvariantCultureIgnoreCase))
            {
                System.Diagnostics.Trace.TraceWarning($"{nameof(CurrentDirectoryOverrider)}: Looks like that current directory has been changed from \"{path}\" to \"{currentDir}\" while we was inside of this overrider scope.");
            }
            Directory.SetCurrentDirectory(currentDirBak);
            Monitor.Exit(syncRoot);
        }
    }
}
