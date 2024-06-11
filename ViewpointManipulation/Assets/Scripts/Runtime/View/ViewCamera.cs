using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Runtime.View
{
    public class ViewCamera : MonoBehaviour
    {
        public ViewPanel viewPanel;
        public Camera cam;

        public int textureWidth = 256;
        public int textureHeight = 256;

        [SerializeField]
        private bool allowGrabbing = false;

        public bool AllowGrabbing
        {
            get => allowGrabbing;
            set
            {
                allowGrabbing = value;
                grabInteractable.enabled = allowGrabbing;
                grabInteractable.colliders[0].enabled = allowGrabbing;
            }
        }

        public XRGrabInteractable grabInteractable;


        [HideInInspector]
        public RenderTexture renderTexture;


        private void Awake()
        {
            grabInteractable.enabled = allowGrabbing;
        }

        public void CreateRenderTexture()
        {
            renderTexture = new RenderTexture(textureWidth, textureHeight, 24);
            renderTexture.enableRandomWrite = true;
            renderTexture.Create();

            cam.targetTexture = renderTexture;
        }

        private void OnDestroy()
        {
            if (renderTexture != null)
            {
                renderTexture.Release();
                renderTexture = null;
            }
        }
    }
}