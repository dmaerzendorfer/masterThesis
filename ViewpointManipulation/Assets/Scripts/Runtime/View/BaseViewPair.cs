using UnityEngine;
using UnityEngine.Events;

namespace Runtime.View
{
    public class BaseViewPair : MonoBehaviour
    {
        public ViewCamera viewCam;
        public ViewPanel viewPanel;

        public UnityEvent onViewPairDeleted = new UnityEvent();
        
        public virtual void Awake()
        {
            //setup render texture of cam and set in panel
            viewCam.CreateRenderTexture();
            viewPanel.SetRenderTexture(viewCam.renderTexture);
            viewPanel.myViewPair = this;
        }

        public virtual void ReceiveSelect()
        {
            //to be overriden by any specific ViewPairs -> could be moved into an interface
        }

        public void DeleteViewPair()
        {
            onViewPairDeleted.Invoke();
            Destroy(viewPanel.gameObject);
            Destroy(viewCam.gameObject);
            Destroy(this.gameObject);
        }
    }
}