using UnityEngine;
using UnityEngine.UI;

namespace K4AdotNet.Samples.Unity
{
    public class ErrorMessage : MonoBehaviour
    {
        public Text Text;

        private void Awake()
        {
            Hide();
        }

        public void Show(string message)
        {
            Text.text = message;
            Text.gameObject.SetActive(true);
        }

        public void Hide()
        {
            Text.gameObject.SetActive(false);
        }
    }
}