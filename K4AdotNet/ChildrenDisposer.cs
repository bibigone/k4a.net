using System;
using System.Collections.Generic;

namespace K4AdotNet
{
    // Helps to track disposable objects which some class creates and controls.
    // Implementation is thread safe.
    internal sealed class ChildrenDisposer : IDisposable
    {
        private readonly LinkedList<IDisposablePlus> trackedChildren = new LinkedList<IDisposablePlus>();

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

        public T? Register<T>(T? child) where T : class, IDisposablePlus
        {
            if (child is null || child.IsDisposed)
                return child;

            lock (trackedChildren)
            {
                trackedChildren.AddLast(child);
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
                for (var node = trackedChildren.First; node != null; node = node.Next)
                {
                    if (ReferenceEquals(node.Value, child))
                    {
                        trackedChildren.Remove(node);
                        return;
                    }
                }
            }

            System.Diagnostics.Trace.TraceWarning($"Cannot find child {child} in list of tracked children.");
        }
    }
}
