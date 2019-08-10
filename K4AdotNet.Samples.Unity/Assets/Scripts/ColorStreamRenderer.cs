using UnityEngine;
using UnityEngine.UI;
using K4AdotNet.Sensor;

namespace K4AdotNet.Samples.Unity
{
    public class ColorStreamRenderer : MonoBehaviour
    {
        public float Alpha;

        private Texture2D _texture;

        // Start is called before the first frame update
        void Start()
        {
            GetComponent<CanvasRenderer>().SetAlpha(Alpha);

            var captureManager = FindObjectOfType<CaptureManager>();
            if (captureManager.IsInitialized)
            {
                _texture = new Texture2D(
                    captureManager.Configuration.ColorResolution.WidthPixels(),
                    captureManager.Configuration.ColorResolution.HeightPixels(),
                    TextureFormat.BGRA32, false);

                GetComponent<RawImage>().texture = _texture;

                captureManager.CaptureReady += CaptureManager_CaptureReady;
            }
        }

        private void OnDestroy()
        {
            var captureManager = FindObjectOfType<CaptureManager>();
            if (captureManager != null) captureManager.CaptureReady -= CaptureManager_CaptureReady;
            Destroy(_texture);
        }

        private void CaptureManager_CaptureReady(object sender, CaptureEventArgs e)
        {
            using (var colorImage = e.Capture.ColorImage)
            {
                if (colorImage != null)
                {
                    _texture.LoadRawTextureData(colorImage.Buffer, colorImage.SizeBytes);
                    _texture.Apply();
                }
            }
        }
    }
}