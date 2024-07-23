using System;
using UnityEngine;

namespace Runtime.View
{
    public class BaseViewPair : MonoBehaviour
    {
        public ViewCamera viewCam;
        public ViewPanel viewPanel;

        public virtual void Awake()
        {
            //setup render texture of cam and set in panel
            viewCam.CreateRenderTexture();
            viewPanel.SetRenderTexture(viewCam.renderTexture);
            viewPanel.myViewPair = this;
        }

        public void DeleteViewPair()
        {
            Destroy(viewPanel.gameObject);
            Destroy(viewCam.gameObject);
            Destroy(this.gameObject);
        }
    }
}