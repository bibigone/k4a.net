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
            currentDirBak = Environment.CurrentDirectory;
            Environment.CurrentDirectory = path;
        }

        public void Dispose()
        {
            Environment.CurrentDirectory = currentDirBak;
            Monitor.Exit(syncRoot);
        }
    }
}
