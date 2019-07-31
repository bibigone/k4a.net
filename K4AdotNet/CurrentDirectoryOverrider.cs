using System;
using System.IO;
using System.Threading;

namespace K4AdotNet
{
    internal sealed class CurrentDirectoryOverrider : IDisposable
    {
        private static readonly object syncRoot = new object();

        private readonly string currentDirBak;

        public CurrentDirectoryOverrider(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            path = Path.GetFullPath(path);
            Monitor.Enter(syncRoot);
            currentDirBak = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(path);
        }

        public void Dispose()
        {
            Directory.SetCurrentDirectory(currentDirBak);
            Monitor.Exit(syncRoot);
        }
    }
}
