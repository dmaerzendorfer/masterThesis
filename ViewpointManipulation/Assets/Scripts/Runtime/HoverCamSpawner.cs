using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

namespace Runtime
{
    public class HoverCamSpawner : MonoBehaviour
    {
        public XRRayInteractor leftController;
        public XRRayInteractor rightController;

        public InputActionReference spawnActionLeft;
        public InputActionReference spawnActionRight;

        private void Start()
        {
            spawnActionLeft.action.performed += OnLeftSpawn;
            spawnActionRight.action.performed += OnRightSpawn;
        }

        private void OnDestroy()
        {
            spawnActionLeft.action.performed -= OnLeftSpawn;
            spawnActionRight.action.performed -= OnRightSpawn;
        }

        private void OnLeftSpawn(InputAction.CallbackContext callbackContext)
        {
            if (leftController.TryGetCurrent3DRaycastHit(out var hit))
            {
                //spawn hoverCam via viewmanager and set its target, remember that its our currently active cam
            }
        }

        private void OnRightSpawn(InputAction.CallbackContext callbackContext)
        {
            if (rightController.TryGetCurrent3DRaycastHit(out var hit))
            {
                //spawn hoverCam via viewmanager and set its target, remember that its our currently active cam
            }
        }
    }
}