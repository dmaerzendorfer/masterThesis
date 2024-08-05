using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime
{
    [RequireComponent(typeof(Renderer))]
    public class PointOfInterest : MonoBehaviour
    {
        public bool useOutline = false;

        [ShowIf("useOutline")]
        public Outline outline;

        public bool IsInView
        {
            get => _isInView;
        }

        [Foldout("Events")]
        public UnityEvent OnIsNowInView = new UnityEvent();
        [Foldout("Events")]
        public UnityEvent OnIsNoLongerInView = new UnityEvent();

        private Renderer _renderer;
        private bool _isInView;

        private void Start()
        {
            _renderer = GetComponent<Renderer>();
        }

        private void Update()
        {
            if (_renderer.isVisible)
            {
                //check if not occluded line of sight to any cam is possible
                //only do the linecast and frustum check if the renderer is visible
                foreach (var cam in Camera.allCameras)
                {
                    CheckIfVisible(cam);
                    //dont check further once its seen
                    if (_isInView) return;
                }
            }
        }

        private void CheckIfVisible(Camera cam)
        {
            //check if in frustum
            var planes = GeometryUtility.CalculateFrustumPlanes(cam);
            if (!GeometryUtility.TestPlanesAABB(planes, _renderer.bounds)) return;

            //check if not occluded
            if (Physics.Linecast(cam.transform.position, transform.position, out var hit))
            {
                if (_isInView)
                    OnIsNoLongerInView.Invoke();
                _isInView = false;
                if (useOutline)
                    outline.enabled = false;
            }
            else
            {
                if (!_isInView)
                    OnIsNowInView.Invoke();
                _isInView = true;
                if (useOutline)
                    outline.enabled = true;
            }
        }
    }
}