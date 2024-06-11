using UnityEngine;

namespace Runtime.View
{
    public class ViewPair : MonoBehaviour
    {
        public ViewCamera viewCam;
        public ViewPanel viewPanel;

        private void Awake()
        {
            //setup render texture of cam and set in panel
            viewCam.CreateRenderTexture();
            viewPanel.SetRenderTexture(viewCam.renderTexture);
        }
    }
}