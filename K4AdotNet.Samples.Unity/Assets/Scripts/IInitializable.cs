using UnityEngine;

namespace K4AdotNet.Samples.Unity
{
    public interface IInitializable
    {
        bool IsInitializationComplete { get; }
        bool IsAvailable { get; }
    }
}
