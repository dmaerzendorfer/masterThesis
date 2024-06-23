using Unity.VRTemplate;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

namespace Runtime.View
{
    [RequireComponent(typeof(XRGrabInteractable))]
    public class ViewPanel : MonoBehaviour
    {
        public ViewCamera viewCamera;
        public RawImage renderImage;
        public BezierCurve curve;

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

        private void Start()
        {
            _viewManager = ViewManager.Instance;
            _grabInteractable = GetComponent<XRGrabInteractable>();

            _curveRenderer = curve.GetComponent<LineRenderer>();

            //hook up the xrgrabinteractable events with the viewManager
            _grabInteractable.selectExited.AddListener(_viewManager.OnViewWindowSelectionExit);
            _grabInteractable.activated.AddListener(_viewManager.OnViewWindowActivate);

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

        public void SetRenderTexture(RenderTexture texture)
        {
            renderImage.texture = texture;
        }
    }
}