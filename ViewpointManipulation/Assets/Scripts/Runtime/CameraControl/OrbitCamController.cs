using Cinemachine;
using NaughtyAttributes;
using UnityEngine;

namespace Runtime.CameraControl
{
    public class OrbitCamController : MonoBehaviour
    {
        public CinemachineVirtualCamera virtualCamera;
        public float moveSpeed = 50f;
        public float heightSpeed = 25f;
        public float radiusSpeed = 50f;

        [BoxGroup("Circle Settings")]
        public LineRenderer circleRenderer;

        [BoxGroup("Circle Settings")]
        public int circleSubdivisions = 100;


        private CinemachineOrbitalTransposer _orbitalTransposer;

        [BoxGroup("Circle Settings")]
        [SerializeField]
        private bool _showCircle = true;

        public bool ShowCircle
        {
            get => _showCircle;
            set
            {
                _showCircle = value;
                if (value)
                {
                    circleRenderer.enabled = true;
                }
                else
                {
                    circleRenderer.enabled = false;
                }
            }
        }

        public float Radius
        {
            get
            {
                if (_orbitalTransposer != null) return _orbitalTransposer.m_FollowOffset.z;
                return 0;
            }
            set
            {
                _orbitalTransposer.m_FollowOffset.z = value;
                DrawCircle();
            }
        }

        public float HeightOffset
        {
            get
            {
                if (_orbitalTransposer != null) return _orbitalTransposer.m_FollowOffset.y;
                return 0;
            }
            set
            {
                _orbitalTransposer.m_FollowOffset.y = value;
                circleRenderer.transform.position = virtualCamera.Follow.position;
            }
        }

        public float XValue
        {
            get
            {
                if (_orbitalTransposer != null) return _orbitalTransposer.m_XAxis.Value;
                return 0;
            }
            set => _orbitalTransposer.m_XAxis.Value = value;
        }

        // Start is called before the first frame update
        void Start()
        {
            _orbitalTransposer = virtualCamera.GetCinemachineComponent<CinemachineOrbitalTransposer>();

            //setup cam
            // Lock the X-axis position
            _orbitalTransposer.m_FollowOffset.x = 0;

            // Set damping to 0 for instant lock without smoothing
            _orbitalTransposer.m_XDamping = 0;

            // Set binding mode to World Space
            _orbitalTransposer.m_BindingMode = CinemachineTransposer.BindingMode.WorldSpace;

            // Optionally disable axis input
            var axis = _orbitalTransposer.m_XAxis;
            axis.m_InputAxisName = string.Empty;
            _orbitalTransposer.m_XAxis = axis; // Clear any assigned input axis

            _orbitalTransposer.m_RecenterToTargetHeading.m_enabled = false;

            if (_showCircle)
                DrawCircle();
        }

        public void SetTarget(Transform targetTransform)
        {
            virtualCamera.Follow = targetTransform;
            virtualCamera.LookAt = targetTransform;
            circleRenderer.transform.position = targetTransform.position;
            if (_showCircle)
            {
                DrawCircle();
            }
        }

        [Button]
        private void DrawCircle()
        {
            float angleStep = 2f * Mathf.PI / circleSubdivisions;
            circleRenderer.positionCount = circleSubdivisions;
            for (int i = 0; i < circleSubdivisions; i++)
            {
                float xPosition = Radius * Mathf.Cos(angleStep * i);
                float zPosition = Radius * Mathf.Sin(angleStep * i);

                Vector3 pointInCircle = new Vector3(xPosition, 0f, zPosition);

                circleRenderer.SetPosition(i, pointInCircle);
            }
        }
    }
}