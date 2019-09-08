using K4AdotNet.Sensor;
using UnityEngine;
using UnityEngine.UI;

namespace K4AdotNet.Samples.Unity.Assets.Scripts
{
    public class DepthStreamRenderer : MonoBehaviour
    {
        public Material DepthToColorMaterial;

        private Texture2D _depthTexture;
        private RenderTexture _renderTexture;

        private void Start()
        {
            var captureManager = FindObjectOfType<CaptureManager>();
            if (captureManager?.IsAvailable == true)
            {
                var frameWidth = captureManager.Configuration.DepthMode.WidthPixels();
                var frameHeight = captureManager.Configuration.DepthMode.HeightPixels();

                _depthTexture = new Texture2D(frameWidth, frameHeight, TextureFormat.R16, false);

                _renderTexture = new RenderTexture(frameWidth, frameHeight, 0);
                _renderTexture.Create();

                GetComponent<RawImage>().texture = _renderTexture;
                GetComponent<AspectRatioFitter>().aspectRatio = (float)frameWidth / frameHeight;

                captureManager.CaptureReady += CaptureManager_CaptureReady;
            }
        }

        private void OnDestroy()
        {
            var captureManager = FindObjectOfType<CaptureManager>();
            if (captureManager != null) captureManager.CaptureReady -= CaptureManager_CaptureReady;
            Destroy(_depthTexture);
            _renderTexture?.Release();
        }

        private void CaptureManager_CaptureReady(object sender, CaptureEventArgs e)
        {
            using (var depthImage = e.Capture.DepthImage)
            {
                if (depthImage != null)
                {
                    _depthTexture.LoadRawTextureData(depthImage.Buffer, depthImage.SizeBytes);
                    _depthTexture.Apply();

                    Graphics.Blit(_depthTexture, _renderTexture, DepthToColorMaterial);
                }
            }
        }
    }
}
