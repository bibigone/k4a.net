using UnityEngine;

namespace K4AdotNet.Samples.Unity
{

    //c#の機能　宣言したものは　継承した先で必ず実装しなければならない
    public interface IInitializable
    {
        //初期化は完了しているか
        //SkeletonProviderクラスで初期化
        bool IsInitializationComplete { get; }

        //Avaiable = 利用可能Initializableクラスの最後でTrueにする
        bool IsAvailable { get; }
    }
}
