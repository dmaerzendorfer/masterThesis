using System;
using NaughtyAttributes;
using Runtime.View.ViewPair;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

namespace Runtime.View.Interactable
{
    [Serializable]
    public class OceActions
    {
        [Tooltip("For moving the oce around and up/down")]
        public InputActionReference uiScroll;

        public InputActionReference inAction;
        public InputActionReference outAction;
    }

    /// <summary>
    /// The Oce camera that can be selected.
    /// </summary>
    public class OceCamInteractable : XRGrabInteractable
    {
        [HorizontalLine(color: EColor.Red)]
        public OceActions actionsLeft;

        public OceActions actionsRight;

        public OceViewPair oceViewPair;
        
        private Vector2 _move;
        private bool _inPressed = false;
        private bool _outPressed = false;


        private void Update()
        {
            if (oceViewPair)
            {
                oceViewPair.orbitCamController.XValue +=
                    oceViewPair.orbitCamController.moveSpeed * _move.x * Time.deltaTime;
                oceViewPair.orbitCamController.HeightOffset +=
                    oceViewPair.orbitCamController.heightSpeed * _move.y * Time.deltaTime;
            }

            if (_outPressed)
            {
                oceViewPair.orbitCamController.Radius +=
                    oceViewPair.orbitCamController.radiusSpeed * Time.deltaTime;
            }

            if (_inPressed)
            {
                oceViewPair.orbitCamController.Radius -=
                    oceViewPair.orbitCamController.radiusSpeed * Time.deltaTime;
            }
        }

        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            base.OnSelectEntered(args);
            oceViewPair.orbitCamController.ShowCircle = true;


            //hook up controls
            var actions = GetCorrectActions(args.interactorObject);
            actions.uiScroll.action.Enable();
            actions.uiScroll.action.performed += OnMove;
            actions.inAction.action.Enable();
            actions.inAction.action.started += OnInPressed;
            actions.inAction.action.canceled += OnInReleased;
            actions.outAction.action.Enable();
            actions.outAction.action.started += OnOutPressed;
            actions.outAction.action.canceled += OnOutReleased;
        }


        protected override void OnSelectExited(SelectExitEventArgs args)
        {
            base.OnSelectExited(args);
            var actions = GetCorrectActions(args.interactorObject);
            //un-hook the controller events for rotation of cam etc.
            actions.uiScroll.action.performed -= OnMove;
            actions.inAction.action.started -= OnInPressed;
            actions.inAction.action.canceled -= OnInReleased;
            actions.outAction.action.started -= OnOutPressed;
            actions.outAction.action.canceled -= OnOutReleased;

            oceViewPair.orbitCamController.ShowCircle = false;
        }

        public void CallOnSelectEntered(SelectEnterEventArgs args)
        {
            OnSelectEntered(args);
        }

        public void CallOnSelectExited(SelectExitEventArgs args)
        {
            OnSelectExited(args);
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
            oceViewPair.orbitCamController.Radius -=
                oceViewPair.orbitCamController.radiusSpeed * Time.deltaTime;
        }

        private void OnInReleased(InputAction.CallbackContext obj)
        {
            _inPressed = false;
            oceViewPair.orbitCamController.Radius -=
                oceViewPair.orbitCamController.radiusSpeed * Time.deltaTime;
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