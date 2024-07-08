using UnityEngine;

namespace Runtime.View
{
    public class ViewCamera : MonoBehaviour
    {
        public ViewPanel viewPanel;
        public Camera cam;

        public int textureWidth = 256;
        public int textureHeight = 256;
        
        

        [HideInInspector]
        public RenderTexture renderTexture;
        

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