using K4AdotNet.BodyTracking;
using System;

namespace K4AdotNet.Samples.Unity
{
    public class SkeletonEventArgs : EventArgs
    {
        public static readonly new SkeletonEventArgs Empty = new SkeletonEventArgs(null);

        public SkeletonEventArgs(Skeleton? value)
        {
            Skeleton = value;
        }

        public Skeleton? Skeleton { get; }
    }
}
