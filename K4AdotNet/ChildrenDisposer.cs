using System;
using System.Collections.Generic;

namespace K4AdotNet
{
    internal sealed class ChildrenDisposer : IDisposable
    {
        private readonly List<IDisposablePlus> trackedChildren = new List<IDisposablePlus>();

        public void Dispose()
        {
            var list = new List<IDisposablePlus>();

            lock (trackedChildren)
            {
                list.AddRange(trackedChildren);
                trackedChildren.Clear();
            }

            foreach (var child in list)
            {
                child.Disposed -= OnChildObjectDisposed;
                child.Dispose();
            }
        }

        public T Register<T>(T child) where T : IDisposablePlus
        {
            if (child == null || child.IsDisposed)
                return child;

            lock (trackedChildren)
            {
                trackedChildren.Add(child);
            }

            child.Disposed += OnChildObjectDisposed;

            return child;
        }

        private void OnChildObjectDisposed(object sender, EventArgs e)
        {
            if (!(sender is IDisposablePlus child))
                throw new ArgumentNullException(nameof(sender));
            lock (trackedChildren)
            {
                child.Disposed -= OnChildObjectDisposed;
                trackedChildren.Remove(child);
            }
        }
    }
}
