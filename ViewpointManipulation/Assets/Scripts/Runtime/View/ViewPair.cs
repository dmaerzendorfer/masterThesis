using Runtime.CameraControl;
using UnityEngine;

namespace Runtime.View
{
    public class ViewPair : MonoBehaviour
    {
        public ViewCamera viewCam;
        public ViewPanel viewPanel;
        public OrbitCamController orbitCamController;

        private void Awake()
        {
            //setup render texture of cam and set in panel
            viewCam.CreateRenderTexture();
            viewPanel.SetRenderTexture(viewCam.renderTexture);
            viewPanel.myViewPair = this;
        }

        public void DeleteViewPair()
        {
            Destroy(viewCam.gameObject);
            Destroy(viewPanel);
            Destroy(this.gameObject);
        }
    }
}