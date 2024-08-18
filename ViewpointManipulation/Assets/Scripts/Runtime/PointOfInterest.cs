using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime
{
    [RequireComponent(typeof(Renderer))]
    public class PointOfInterest : MonoBehaviour
    {
        public bool useOutline = false;
        public float minDistance = 1f;

        public bool showDebugRay = false;

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

        private void FixedUpdate()
        {
            //check if not occluded line of sight to any cam is possible
            //only do the linecast and frustum check if the renderer is visible
            bool tempInView = false;
            //need to go through all the cameras to know for certain if poi is no longer in view or not
            foreach (var cam in Camera.allCameras)
            {
                if (cam == null) continue;
                tempInView |= CheckIfVisible(cam);
                //dont check further once its seen
                if (tempInView) break;
            }

            if (!_isInView && tempInView)
            {
                Debug.Log("Now in view");
                OnIsNowInView.Invoke();
            }

            if (_isInView && !tempInView)
            {
                Debug.Log("no longer in view");
                OnIsNoLongerInView.Invoke();
            }


            _isInView = tempInView;
            if (useOutline)
                outline.enabled = _isInView;
        }

        private bool CheckIfVisible(Camera cam)
        {
            //check if even close enough to cam
            if (showDebugRay)
            {
                var dir = (_renderer.bounds.center - cam.transform.position).normalized;
                Debug.DrawRay(transform.position, -dir * minDistance, Color.yellow);
            }

            if (Vector3.Distance(_renderer.bounds.center, cam.transform.position) <= minDistance)
            {
                //check if in frustum
                var planes = GeometryUtility.CalculateFrustumPlanes(cam);
                if (GeometryUtility.TestPlanesAABB(planes, _renderer.bounds))
                {
                    //check if not occluded

                    if (!Physics.Linecast(_renderer.bounds.center, cam.transform.position, out var hit,
                            ~LayerMask.GetMask("CamModel")))
                    {
                        // Debug.Log("didnt hit a collider");
                        return true;
                    }
                    // Debug.Log("hit a collider");
                }
            }

            return false;
        }
    }
}