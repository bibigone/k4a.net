using K4AdotNet.Sensor;
using System;
using UnityEngine;

namespace K4AdotNet.Samples.Unity
{
    public class CaptureManager : MonoBehaviour, IInitializable
    {
        private Device _device;

        public bool IsInitializationComplete { get; private set; }
        //プロパティアクセサ = 式の結果を割り当てるか返すだけの1 行のステートメントで構成
        //プロパティは、式形式のメンバとして実装できる
        //式本体の定義は、=> 後、プロパティに割り当てるかプロパティから取得するための式を続けて構成
        // nullじゃなかったらtrueが入る
        public bool IsAvailable => _device != null;
        public DeviceConfiguration Configuration { get; private set; }
        public Calibration Calibration { get; private set; }

        // event = C#のコールバック　登録する
        // += 関数を登録 -= 関数を削除
        // delegate = 自分で作った関数
        public event EventHandler<CaptureEventArgs> CaptureReady;

        private void Awake()
        {
            if (Device.TryOpen(out _device))
            {
                //newして中かっこすると構造体とクラス初期化
                Configuration = new DeviceConfiguration
                {
                    ColorResolution = ColorResolution.R720p,
                    ColorFormat = ImageFormat.ColorBgra32,
                    DepthMode = DepthMode.NarrowViewUnbinned,
                    CameraFps = FrameRate.Thirty,
                };
                _device.GetCalibration(Configuration.DepthMode, Configuration.ColorResolution, out var calibration);
                Calibration = calibration;
            }
            else
            {
                Debug.Log("Cannot open device");
            }

            IsInitializationComplete = true;
        }

        private void Start()
        {
            if (IsAvailable)
            {
                _device.StartCameras(Configuration);
            }
        }

        private void Update()
        {
            if (IsAvailable)
            {
                //out演算子 = 参照渡し 引数で変数を宣言できる
                if (_device.TryGetCapture(out var capture))
                {
                    //勝手に呼ばれるDispose
                    using (capture)
                    {
                        //Invoke = 自分自身　実行する
                        CaptureReady?.Invoke(this, new CaptureEventArgs(capture));
                    }
                }
            }
        }

        private void OnDestroy()
        {
            // ? = 変数がnullじゃなかったら実行される
            _device?.StopCameras();
            _device?.Dispose();
            _device = null;
        }
    }
}