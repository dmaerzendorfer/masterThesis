using Cinemachine;
using UnityEngine;

namespace Runtime.CameraControl
{
    public class OrbitCamController : MonoBehaviour
    {
        public CinemachineVirtualCamera virtualCamera;

        private CinemachineOrbitalTransposer _orbitalTransposer;


        public float Radius
        {
            get
            {
                if (_orbitalTransposer != null) return _orbitalTransposer.m_FollowOffset.z;
                return 0;
            }
            set => _orbitalTransposer.m_FollowOffset.z = value;
        }

        public float HeightOffset
        {
            get
            {
                if (_orbitalTransposer != null) return _orbitalTransposer.m_FollowOffset.y;
                return 0;
            }
            set => _orbitalTransposer.m_FollowOffset.y = value;
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
        }

        public void SetTarget(Transform targetTransform)
        {
            virtualCamera.Follow = targetTransform;
            virtualCamera.LookAt = targetTransform;
        }
    }
}