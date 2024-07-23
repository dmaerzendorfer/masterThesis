using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

namespace Runtime.View
{
    /// <summary>
    /// The object in center
    /// </summary>
    public class OceInteractable : XRGrabInteractable
    {
        [HorizontalLine(color: EColor.Red)]
        public OceActions actionsLeft;

        public OceActions actionsRight;

        [HorizontalLine(color: EColor.Red)]
        public Transform lookAtTarget;

        // [HorizontalLine(color: EColor.Red)]
        // public OceHandle oceHandlePrefab;
        //
        // [HorizontalLine(color: EColor.Red)]
        // public float moveDeadzone = 0.1f;

        // [HorizontalLine(color: EColor.Red)]
        // public float viewPanelDistance = 3f;

        // private OceHandle _currentHandle;
        private ViewManager _viewManager;
        private OceViewPair _mostRecentOceViewPair = null;

        private Vector2 _move;
        private bool _inPressed = false;
        private bool _outPressed = false;

        private void Start()
        {
            _viewManager = ViewManager.Instance;
        }

        private void Update()
        {
            if (_mostRecentOceViewPair)
            {
                _mostRecentOceViewPair.orbitCamController.XValue +=
                    _mostRecentOceViewPair.orbitCamController.moveSpeed * _move.x * Time.deltaTime;
                _mostRecentOceViewPair.orbitCamController.HeightOffset +=
                    _mostRecentOceViewPair.orbitCamController.heightSpeed * _move.y * Time.deltaTime;
            }

            if (_outPressed)
            {
                _mostRecentOceViewPair.orbitCamController.Radius +=
                    _mostRecentOceViewPair.orbitCamController.radiusSpeed * Time.deltaTime;
            }

            if (_inPressed)
            {
                _mostRecentOceViewPair.orbitCamController.Radius -=
                    _mostRecentOceViewPair.orbitCamController.radiusSpeed * Time.deltaTime;
            }
        }

        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            base.OnSelectEntered(args);

            //spawn orbit cam and set lookat target
            _mostRecentOceViewPair = _viewManager.SpawnOce();
            if (_mostRecentOceViewPair == null)
                return;
            _mostRecentOceViewPair.orbitCamController.SetTarget(lookAtTarget);
            var camInteractable = _mostRecentOceViewPair.orbitCamController.GetComponent<OceCamInteractable>();
            args.interactableObject = camInteractable;
            camInteractable.CallOnSelectEntered(args);
            // camInteractable.selectEntered.Invoke(args);

            // _mostRecentViewPair.orbitCamController.ShowCircle = true;

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

            // //hook up controls
            // var actions = GetCorrectActions(args.interactorObject);
            // actions.uiScroll.action.Enable();
            // actions.uiScroll.action.performed += OnMove;
            // actions.inAction.action.Enable();
            // actions.inAction.action.started += OnInPressed;
            // actions.inAction.action.canceled += OnInReleased;
            // actions.outAction.action.Enable();
            // actions.outAction.action.started += OnOutPressed;
            // actions.outAction.action.canceled += OnOutReleased;

            //move the view panel to an opportune position
            _viewManager.AdjustNewViewPanelPosition(_mostRecentOceViewPair);
            // var mainCam = Camera.main;
            // RaycastHit hit;
            // if (Physics.Raycast(mainCam.transform.position, mainCam.transform.forward, out hit, viewPanelDistance))
            // {
            //     _mostRecentOceViewPair.viewPanel.transform.position = hit.point;
            // }
            // else
            // {
            //     _mostRecentOceViewPair.viewPanel.transform.position =
            //         mainCam.transform.position + mainCam.transform.forward * viewPanelDistance;
            // }
        }


        protected override void OnSelectExited(SelectExitEventArgs args)
        {
            base.OnSelectExited(args);
            var camInteractable = _mostRecentOceViewPair.orbitCamController.GetComponent<OceCamInteractable>();
            args.interactableObject = camInteractable;
            camInteractable.CallOnSelectExited(args);

            // var actions = GetCorrectActions(args.interactorObject);
            // // Destroy(_currentHandle.gameObject);
            // //un-hook the controller events for rotation of cam etc.
            // //actions.uiScroll.action.Disable();
            // actions.uiScroll.action.performed -= OnMove;
            // //actions.inAction.action.Disable();
            // actions.inAction.action.started -= OnInPressed;
            // actions.inAction.action.canceled -= OnInReleased;
            // //actions.outAction.action.Disable();
            // actions.outAction.action.started -= OnOutPressed;
            // actions.outAction.action.canceled -= OnOutReleased;
            //
            // _mostRecentViewPair.orbitCamController.ShowCircle = false;
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

        private void OnMove(InputAction.CallbackContext callbackContext)
        {
            _move = callbackContext.ReadValue<Vector2>();
        }


        private void OnInPressed(InputAction.CallbackContext obj)
        {
            _inPressed = true;
            _mostRecentOceViewPair.orbitCamController.Radius -=
                _mostRecentOceViewPair.orbitCamController.radiusSpeed * Time.deltaTime;
        }

        private void OnInReleased(InputAction.CallbackContext obj)
        {
            _inPressed = false;
            _mostRecentOceViewPair.orbitCamController.Radius -=
                _mostRecentOceViewPair.orbitCamController.radiusSpeed * Time.deltaTime;
        }

        private void OnOutPressed(InputAction.CallbackContext obj)
        {
            _outPressed = true;
        }

        private void OnOutReleased(InputAction.CallbackContext obj)
        {
            _outPressed = false;
        }
    }
}