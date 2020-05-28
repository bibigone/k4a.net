using System.Collections;
using UnityEngine;

namespace K4AdotNet.Samples.Unity
{
    public class ApplicationController : MonoBehaviour
    {
        private ErrorMessage _errorMessage;
        private GameObject _skeleton;
        private GameObject _character;
        private GameObject _modes;

        private void Awake()
        {
            _errorMessage = FindObjectOfType<ErrorMessage>();
            _skeleton = GetComponentInChildren<SkeletonRenderer>(includeInactive: true)?.gameObject;
            _character = GetComponentInChildren<CharacterAnimator>(includeInactive: true)?.gameObject;
            _modes = GameObject.Find("Modes");
        }

        private IEnumerator Start()
        {
            _modes.SetActive(false);

            var captureManager = FindObjectOfType<CaptureManager>();
            yield return new WaitUntil(() => captureManager?.IsInitializationComplete != false);
            if (captureManager?.IsAvailable != true)
            {
                _errorMessage.Show("Azure Kinect is not connected or recognized");
                yield break;
            }

            var skeletonProvider = FindObjectOfType<SkeletonProvider>();
            yield return new WaitUntil(() => skeletonProvider?.IsInitializationComplete != false);
            if (skeletonProvider?.IsAvailable != true)
            {
                _errorMessage.Show(
                    "Cannot initialize Azure Kinect Body Tracking runtime.\n" +
                    "Make sure that there are the following files in Assets\\Plugins\\K4AdotNet folder:\n" +
                    "1. k4abt.dll\n2. dnn_model_2_0.onnx\n3. cudnn64_7.dll\n4. cublas64_100.dll\n5. cudart64_100.dll\n6. onnxruntime.dll\n7. vcomp140.dll");
                yield break;
            }

            _modes.SetActive(true);
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
            }
        }

        public void OnSkeletonModeToggled(bool isActive)
        {
            _skeleton?.SetActive(isActive);
        }

        public void OnCharacterModeToggled(bool isActive)
        {
            _character?.SetActive(isActive);
        }
    }
}
