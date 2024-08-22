using Runtime.View.Panel;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.View.ViewPair
{
    public abstract class BaseViewPair : MonoBehaviour
    {
        public ViewCamera viewCam;
        public BaseViewPanel basePanel;

        public UnityEvent onViewPairDeleted = new UnityEvent();
        
        public virtual void Awake()
        {
            //setup render texture of cam and set in panel
            viewCam.CreateRenderTexture();
            basePanel.SetRenderTexture(viewCam.renderTexture);
            basePanel.myViewPair = this;
        }

        public abstract void ReceiveSelect();

        public virtual void DeleteViewPair()
        {
            onViewPairDeleted.Invoke();
            Destroy(basePanel.gameObject);
            Destroy(viewCam.gameObject);
            Destroy(this.gameObject);
        }
    }
}