using UnityEngine;

namespace K4AdotNet.Samples.Unity
{
    public class ApplicationController : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetKey("escape"))
            {
                Application.Quit();
            }
        }
    }
}
