using UnityEngine;

namespace K4AdotNet.Samples.Unity
{

    //c#の機能　宣言したものは　継承した先で必ず実装しなければならない
    public interface IInitializable
    {
        bool IsInitializationComplete { get; }
        bool IsAvailable { get; }
    }
}
