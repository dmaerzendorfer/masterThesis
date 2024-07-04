using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

namespace Runtime.View
{
    [Serializable]
    public class OceActions
    {
        [Tooltip("For moving the oce around and up/down")]
        public InputActionReference uiScroll;

        public InputActionReference inAction;
        public InputActionReference outAction;
    }

    public class OceInteractable : XRGrabInteractable
    {
        [HorizontalLine(color: EColor.Red)]
        public OceActions actionsLeft;

        public OceActions actionsRight;

        [Tooltip("The reference to the action of moving the OCE Camera.")]
        public InputActionReference uiScrollRight;

        public InputActionReference uiScrollLeft;

        [HorizontalLine(color: EColor.Red)]
        public Transform lookAtTarget;

        // [HorizontalLine(color: EColor.Red)]
        // public OceHandle oceHandlePrefab;
        //
        // [HorizontalLine(color: EColor.Red)]
        // public float moveDeadzone = 0.1f;

        [HorizontalLine(color: EColor.Red)]
        public float viewPanelDistance = 3f;

        // private OceHandle _currentHandle;
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

            _mostRecentViewPair.orbitCamController.ShowCircle = true;


            //setup oce handle
            // _currentHandle = Instantiate(oceHandlePrefab, args.interactorObject.transform.position,
            //     Quaternion.identity);
            // _currentHandle.controllerTransform = args.interactorObject.transform;
            //
            // //set up the handles plane
            // _currentHandle.planeOrigin = args.interactorObject.transform.position;
            // _currentHandle.planeNormal =
            //     (Camera.main.transform.position - lookAtTarget.position).normalized;
            // _currentHandle.SetDiameter(moveDeadzone * 2);

            //hook up controls
            var actions = GetCorrectActions(args.interactorObject);
            actions.uiScroll.action.Enable();
            actions.uiScroll.action.performed += OnMove;
            actions.inAction.action.Enable();
            actions.inAction.action.performed += OnIn;
            actions.outAction.action.Enable();
            actions.outAction.action.performed += OnOut;


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
            var actions = GetCorrectActions(args.interactorObject);
            // Destroy(_currentHandle.gameObject);
            //un-hook the controller events for rotation of cam etc.
            actions.uiScroll.action.Disable();
            actions.uiScroll.action.performed -= OnMove;
            actions.inAction.action.Disable();
            actions.inAction.action.performed -= OnIn;
            actions.outAction.action.Disable();
            actions.outAction.action.performed -= OnOut;

            _mostRecentViewPair.orbitCamController.ShowCircle = false;
        }

        private OceActions GetCorrectActions(IXRSelectInteractor interactor)
        {
            if (interactor.transform.parent.name.Contains("Left"))
            {
                return actionsLeft;
            }
            else
            {
                return actionsRight;
            }
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
            _mostRecentViewPair.orbitCamController.HeightOffset +=
                _mostRecentViewPair.orbitCamController.heightSpeed * val.y * Time.deltaTime;
        }

        private void OnIn(InputAction.CallbackContext obj)
        {
            _mostRecentViewPair.orbitCamController.Radius -=
                _mostRecentViewPair.orbitCamController.radiusSpeed * Time.deltaTime;
        }

        private void OnOut(InputAction.CallbackContext obj)
        {
            _mostRecentViewPair.orbitCamController.Radius +=
                _mostRecentViewPair.orbitCamController.radiusSpeed * Time.deltaTime;
        }
    }
}