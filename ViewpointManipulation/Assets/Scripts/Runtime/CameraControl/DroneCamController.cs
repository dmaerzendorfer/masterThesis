using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Runtime.CameraControl
{
    [Serializable]
    public class DroneActions
    {
        //left joystick
        public InputActionReference moveAction;

        //right joystick, x = yaw, y = pitch
        public InputActionReference yawPitchAction;

        //B button
        public InputActionReference upAction;

        //A button
        public InputActionReference downAction;

        public void EnableAllActions()
        {
            moveAction.action.Enable();
            yawPitchAction.action.Enable();
            upAction.action.Enable();
            downAction.action.Enable();
        }

        public void DisableAllActions()
        {
            moveAction.action.Disable();
            yawPitchAction.action.Disable();
            upAction.action.Disable();
            downAction.action.Disable();
        }
    }

    public class DroneCamController : MonoBehaviour
    {
        public float moveSpeed = 50f;
        public float pitchSpeed = 50f;
        public float yawSpeed = 50f;
        public float heightSpeed = 25f;

        public DroneActions inputActions;

        public Outline modelOutline;

        [SerializeField]
        private bool _isSelected = false;

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                if (_isSelected)
                {
                    modelOutline.enabled = true;
                    inputActions.EnableAllActions();
                }
                else
                {
                    modelOutline.enabled = false;
                    inputActions.DisableAllActions();
                    ResetAnyInput();
                }
            }
        }

        private Transform _mainCamTransform;

        private Vector2 _moveInput;
        private Vector2 _yawPitchInput;
        private bool _upInput = false;
        private bool _downInput = false;

        private void Awake()
        {
            _mainCamTransform = Camera.main.transform;
            //for setting the outline on/off if stuff is already set in the inspector
            IsSelected = _isSelected;

            //hook up the actions
            inputActions.moveAction.action.performed += OnMove;
            inputActions.yawPitchAction.action.performed += OnYawPitch;
            inputActions.upAction.action.started += OnUpPressed;
            inputActions.upAction.action.canceled += OnUpReleased;
            inputActions.downAction.action.started += OnDownPressed;
            inputActions.downAction.action.canceled += OnDownReleased;
        }

        private void OnDestroy()
        {
            //un-hook up the actions
            inputActions.moveAction.action.performed -= OnMove;
            inputActions.yawPitchAction.action.performed -= OnYawPitch;
            inputActions.upAction.action.started -= OnUpPressed;
            inputActions.upAction.action.canceled -= OnUpReleased;
            inputActions.downAction.action.started -= OnDownPressed;
            inputActions.downAction.action.canceled -= OnDownReleased;
        }

        private void Update()
        {
            //apply the input
            if (!IsSelected) return;

            //handle up/down
            if (_upInput)
                transform.position += _mainCamTransform.up * (heightSpeed * Time.deltaTime);
            if (_downInput)
                transform.position -= _mainCamTransform.up * (heightSpeed * Time.deltaTime);

            //handle yaw/pitch
            var currentEulerAngles = transform.eulerAngles;
            //yaw
            currentEulerAngles.y += _yawPitchInput.x * yawSpeed * Time.deltaTime;
            //pitch
            currentEulerAngles.x += _yawPitchInput.y * pitchSpeed * Time.deltaTime;
            transform.eulerAngles = currentEulerAngles;

            //handle movement (from the mainCams perspective)
            Vector3 translation = _mainCamTransform.forward * _moveInput.y +
                                  _mainCamTransform.right * _moveInput.x;
            transform.position += translation * (moveSpeed * Time.deltaTime);
        }

        private void ResetAnyInput()
        {
            _moveInput = Vector2.zero;
            _yawPitchInput = Vector2.zero;
            _downInput = false;
            _upInput = false;
        }

        private void OnMove(InputAction.CallbackContext callbackContext)
        {
            _moveInput = callbackContext.ReadValue<Vector2>();
        }

        private void OnYawPitch(InputAction.CallbackContext callbackContext)
        {
            _yawPitchInput = callbackContext.ReadValue<Vector2>();
        }

        private void OnUpPressed(InputAction.CallbackContext callbackContext)
        {
            _upInput = true;
        }

        private void OnUpReleased(InputAction.CallbackContext callbackContext)
        {
            _upInput = false;
        }

        private void OnDownPressed(InputAction.CallbackContext callbackContext)
        {
            _downInput = true;
        }

        private void OnDownReleased(InputAction.CallbackContext callbackContext)
        {
            _downInput = false;
        }
    }
}