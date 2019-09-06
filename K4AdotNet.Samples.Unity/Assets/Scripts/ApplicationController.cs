using System;
using System.Collections;
using UnityEngine;

namespace K4AdotNet.Samples.Unity
{
    public class ApplicationController : MonoBehaviour
    {
        private GameObject _skeleton;
        private GameObject _character;
        private GameObject _modes;

        private void Awake()
        {
            _skeleton = GetComponentInChildren<SkeletonRenderer>(includeInactive: true)?.gameObject;
            _character = GetComponentInChildren<CharacterAnimator>(includeInactive: true)?.gameObject;
            _modes = GameObject.Find("Modes");
        }

        public bool IsInitialized { get; private set; }
        public bool IsBodyTrackingAvailable { get; private set; }

        private IEnumerator Start()
        {
            _modes.SetActive(false);

            var attempt = 1;
            do
            {
                yield return new WaitForSeconds(2);

                try
                {
                    IsBodyTrackingAvailable = Sdk.TryInitializeBodyTrackingRuntime(out var message);
                    if (!IsBodyTrackingAvailable)
                    {
                        Debug.Log($"Cannot initialize body tracking: {message}");
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Exception on {nameof(Sdk.TryInitializeBodyTrackingRuntime)}\r\n{ex}");
                }
            } while (!IsBodyTrackingAvailable && ++attempt <= 3);

            IsInitialized = true;
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
