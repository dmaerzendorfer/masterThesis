using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using Runtime.View.Manager;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

namespace Runtime.CameraControl
{
    public enum DroneMovementMode
    {
        UserRelative = 0,
        DroneRelative = 1 << 0
    }

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
        public float moveSpeed = 25f;
        public float pitchSpeed = 75f;
        public float yawSpeed = 75f;
        public float heightSpeed = 10f;

        public DroneActions inputActions;

        public DroneMovementMode movementMode = DroneMovementMode.UserRelative;

        public Outline modelOutline;

        [SerializeField]
        private bool _isSelected = false;

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (value == _isSelected) return;
                _isSelected = value;
                if (_isSelected)
                {
                    modelOutline.enabled = true;
                    //enable drone actions
                    inputActions.EnableAllActions();
                    //disable default vr locomotion actions
                    SetLocomotionEnabled(false);
                    onSelected.Invoke(this);
                }
                else
                {
                    modelOutline.enabled = false;
                    //disable drone actions
                    inputActions.DisableAllActions();
                    //enable default vr locomotion actions
                    SetLocomotionEnabled(true);
                    onUnselected.Invoke(this);

                    ResetAnyInput();
                }
            }
        }

        [Foldout("Events")]
        public UnityEvent<DroneCamController> onMovementModeChange = new UnityEvent<DroneCamController>();

        [Foldout("Events")]
        public UnityEvent<DroneCamController> onSelected = new UnityEvent<DroneCamController>();

        [Foldout("Events")]
        public UnityEvent<DroneCamController> onUnselected = new UnityEvent<DroneCamController>();


        private Transform _mainCamTransform;

        private Vector2 _moveInput;
        private Vector2 _yawPitchInput;
        private bool _upInput = false;
        private bool _downInput = false;

        private LocomotionSystem _locomotionSystem;

        private List<ActionBasedControllerManager> _locomotionControllerManagers =
            new List<ActionBasedControllerManager>();

        private ViewManager _viewManager;

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

        private void Start()
        {
            _viewManager = ViewManager.Instance;
        }

        private void OnDestroy()
        {
            //make sure to enable locomotion again if its disabled
            if (IsSelected)
            {
                SetLocomotionEnabled(true);
            }

            //un-hook up the actions
            inputActions.moveAction.action.performed -= OnMove;
            inputActions.yawPitchAction.action.performed -= OnYawPitch;
            inputActions.upAction.action.started -= OnUpPressed;
            inputActions.upAction.action.canceled -= OnUpReleased;
            inputActions.downAction.action.started -= OnDownPressed;
            inputActions.downAction.action.canceled -= OnDownReleased;

            //setting selected in order to trigger the unselect event
            IsSelected = false;
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
            if (movementMode == DroneMovementMode.UserRelative)
            {
                var forward = _mainCamTransform.forward;
                forward.y = 0;
                forward.Normalize();
                var right = _mainCamTransform.right;
                right.y = 0;
                right.Normalize();
                Vector3 translation = forward * _moveInput.y +
                                      right * _moveInput.x;
                transform.position += translation * (moveSpeed * Time.deltaTime);
            }
            //or from drone perspective
            else if (movementMode == DroneMovementMode.DroneRelative)
            {
                Vector3 translation = transform.forward * _moveInput.y +
                                      transform.right * _moveInput.x;
                transform.position += translation * (moveSpeed * Time.deltaTime);
            }
        }

        public void SetLocomotionEnabled(bool enabled)
        {
            if (TryToFindLocomotion())
            {
                _locomotionSystem.gameObject.SetActive(enabled);
                foreach (var c in _locomotionControllerManagers)
                {
                    c.enabled = enabled;
                }
            }
        }

        public string ToggleMovementMode()
        {
            if (movementMode == DroneMovementMode.DroneRelative)
            {
                movementMode = DroneMovementMode.UserRelative;
                onMovementModeChange.Invoke(this);
                return "User Relative";
            }
            else
            {
                movementMode = DroneMovementMode.DroneRelative;
                onMovementModeChange.Invoke(this);
                return "Drone Relative";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>whether locomotion system was found</returns>
        private bool TryToFindLocomotion()
        {
            if (_locomotionSystem != null) return true;
            _locomotionSystem = FindObjectOfType<LocomotionSystem>();
            _locomotionControllerManagers = FindObjectsOfType<ActionBasedControllerManager>().ToList();
            return _locomotionSystem != null;
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