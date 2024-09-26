using Runtime.View.Manager;
using Runtime.View.ViewPair;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

namespace Runtime
{
    public class HoverCamSpawner : MonoBehaviour
    {
        public bool isSpawningEnabled = true;
        public XRRayInteractor rightController;
        public InputActionReference spawnActionRight;
        public ViewManager viewManager;
        public LayerMask layerMask;

        private void Start()
        {
            spawnActionRight.action.Enable();
            spawnActionRight.action.performed += OnRightSpawn;
        }

        private void OnDestroy()
        {
            spawnActionRight.action.Disable();
            spawnActionRight.action.performed -= OnRightSpawn;
        }

        private void OnRightSpawn(InputAction.CallbackContext callbackContext)
        {
            if (!isSpawningEnabled) return;
            if (rightController.TryGetCurrent3DRaycastHit(out var hit))
            {
                if (!layerMask.Contains(hit.collider.gameObject.layer)) return;

                //spawn hoverCam via viewmanager and set its target
                var hover = (HoverViewPair)viewManager.SpawnViewPair();
                hover.hoverCamController.target = hit.collider;
                hover.hoverCamController.currentLookAt = hit.point;
                hover.hoverCamController.SetInitialPos();
            }
        }
    }
}