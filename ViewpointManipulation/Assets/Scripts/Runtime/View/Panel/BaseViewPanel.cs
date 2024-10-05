using Runtime.View.Manager;
using Runtime.View.ViewPair;
using TMPro;
using Unity.VRTemplate;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

namespace Runtime.View.Panel
{
    [RequireComponent(typeof(XRGrabInteractable))]
    public class BaseViewPanel : MonoBehaviour
    {
        public RawImage renderImage;
        public BezierCurve curve;

        public TextMeshProUGUI panelText;
        public TextMeshProUGUI selectText;


        [HideInInspector]
        public BaseViewPair myViewPair;

        private LineRenderer _curveRenderer;


        public bool IsInHud
        {
            get => _isInHud;
            set
            {
                _isInHud = value;
                curve.enabled = !_isInHud;
                _curveRenderer.enabled = curve.enabled;
            }
        }

        private bool _isInHud = false;
        private XRGrabInteractable _grabInteractable;

        private ViewManager _viewManager;

        public virtual void Start()
        {
            _viewManager = ViewManager.Instance;
            _grabInteractable = GetComponent<XRGrabInteractable>();

            _curveRenderer = curve.GetComponent<LineRenderer>();

            //hook up the xrgrabinteractable events with the viewManager
            _grabInteractable.selectExited.AddListener(_viewManager.OnViewWindowSelectionExit);
            _grabInteractable.activated.AddListener(_viewManager.OnViewPanelActivate);

            _grabInteractable.hoverEntered.AddListener((x) =>
            {
                if (IsInHud)
                {
                    curve.enabled = true;
                    _curveRenderer.enabled = true;
                }
            });
            _grabInteractable.hoverExited.AddListener((x) =>
            {
                if (IsInHud)
                {
                    curve.enabled = false;
                    _curveRenderer.enabled = false;
                }
            });
        }

        public void DeleteViewPair()
        {
            if (myViewPair)
                myViewPair.DeleteViewPair();
        }

        public void SelectViewCam()
        {
            if (myViewPair)
                myViewPair.ReceiveSelect();
        }


        public void SetRenderTexture(RenderTexture texture)
        {
            renderImage.texture = texture;
        }
    }
}