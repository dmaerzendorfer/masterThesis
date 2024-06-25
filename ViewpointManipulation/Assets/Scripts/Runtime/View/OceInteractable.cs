using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

namespace Runtime.View
{
    public class OceInteractable : XRGrabInteractable
    {
        [HorizontalLine(color: EColor.Red)]
        [Tooltip("The reference to the action of moving the OCE Camera.")]
        public InputActionReference uiScrollRight;

        public InputActionReference uiScrollLeft;

        [HorizontalLine(color: EColor.Red)]
        public Transform lookAtTarget;

        [HorizontalLine(color: EColor.Red)]
        public OceHandle oceHandlePrefab;

        [HorizontalLine(color: EColor.Red)]
        public float moveDeadzone = 0.1f;

        [HorizontalLine(color: EColor.Red)]
        public float viewPanelDistance = 3f;

        private OceHandle _currentHandle;
        private ViewManager _viewManager;
        private ViewPair _mostRecentViewPair = null;

        private void Start()
        {
            _viewManager = ViewManager.Instance;
        }

        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            base.OnSelectEntered(args);

            //spawn orbit cam and set lookat target
            _mostRecentViewPair = _viewManager.SpawnOce();
            if (_mostRecentViewPair == null)
                return;
            _mostRecentViewPair.orbitCamController.SetTarget(lookAtTarget);

            //setup oce handle
            _currentHandle = Instantiate(oceHandlePrefab, args.interactorObject.transform.position,
                Quaternion.identity);
            _currentHandle.lineTarget = args.interactorObject.transform;
            _currentHandle.planeOrigin = args.interactorObject.transform;
            _currentHandle.planeLookingTowards = _currentHandle.transform;
            _currentHandle.objectInCenter = lookAtTarget;
            _currentHandle.SetDiameter(moveDeadzone * 2);

            var actionReference = GetCorrectScrollAction(args.interactorObject);
            actionReference.action.Enable();
            actionReference.action.performed += OnMove;

            //move the view panel to an opportune position
            var mainCam = Camera.main;
            RaycastHit hit;
            if (Physics.Raycast(mainCam.transform.position, mainCam.transform.forward, out hit, viewPanelDistance))
            {
                _mostRecentViewPair.viewPanel.transform.position = hit.point;
            }
            else
            {
                _mostRecentViewPair.viewPanel.transform.position =
                    mainCam.transform.position + mainCam.transform.forward * viewPanelDistance;
            }
        }

        protected override void OnSelectExited(SelectExitEventArgs args)
        {
            base.OnSelectExited(args);
            var actionReference = GetCorrectScrollAction(args.interactorObject);
            Destroy(_currentHandle.gameObject);
            //un-hook the controller events for rotation of cam etc.
            actionReference.action.Disable();
            actionReference.action.performed -= OnMove;
        }

        private InputActionReference GetCorrectScrollAction(IXRSelectInteractor interactor)
        {
            if (interactor.transform.parent.name.Contains("Left"))
            {
                return uiScrollLeft;
            }
            else
            {
                return uiScrollRight;
            }
        }

        private void OnMove(InputAction.CallbackContext callbackContext)
        {
            Vector2 val = callbackContext.ReadValue<Vector2>();
            _mostRecentViewPair.orbitCamController.XValue +=
                _mostRecentViewPair.orbitCamController.moveSpeed * val.x * Time.deltaTime;
        }
    }
}