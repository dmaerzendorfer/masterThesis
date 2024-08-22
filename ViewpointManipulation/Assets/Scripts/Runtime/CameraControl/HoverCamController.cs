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
    [Serializable]
    public class HoverActions
    {
        public InputActionReference moveAction;

        public InputActionReference zoomInAction;

        public InputActionReference zoomOutAction;

        public void EnableAllActions()
        {
            moveAction.action.Enable();
            zoomInAction.action.Enable();
            zoomOutAction.action.Enable();
        }

        public void DisableAllActions()
        {
            moveAction.action.Disable();
            zoomInAction.action.Disable();
            zoomOutAction.action.Disable();
        }
    }

    // based on https://dl.acm.org/doi/pdf/10.1145/1053427.1053439
    public class HoverCamController : MonoBehaviour
    {
        public float moveSpeed = 25f;
        public float zoomSpeed = 5f;

        public HoverActions inputActions;

        public float distanceToObject = 1f;
        public float minDistanceToObject = .5f;

        public Collider target;
        public Vector3 currentLookAt;

        public Outline modelOutline;

        [SerializeField]
        private bool _isSelected = false;

        private LocomotionSystem _locomotionSystem;

        private List<ActionBasedControllerManager> _locomotionControllerManagers =
            new List<ActionBasedControllerManager>();


        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (value == _isSelected) return;
                if (value == true)
                {
                    //unselect any currently selected hoverCams
                    ViewManager.Instance.hoverViewModeHandler.viewConfigs.ForEach(x =>
                    {
                        if (x.instance)
                            x.instance.hoverCamController.IsSelected = false;
                    });
                }

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
        public UnityEvent<HoverCamController> onSelected = new UnityEvent<HoverCamController>();

        [Foldout("Events")]
        public UnityEvent<HoverCamController> onUnselected = new UnityEvent<HoverCamController>();


        private bool _inPressed = false;
        private bool _outPressed = false;
        private Vector2 _moveInput = Vector2.zero;

        private void Start()
        {
            inputActions.moveAction.action.performed += OnMove;

            inputActions.zoomInAction.action.started += OnInPressed;
            inputActions.zoomInAction.action.canceled += OnInReleased;
            inputActions.zoomOutAction.action.started += OnOutPressed;
            inputActions.zoomOutAction.action.canceled += OnOutReleased;
        }

        private void OnDestroy()
        {
            inputActions.moveAction.action.performed -= OnMove;
            inputActions.zoomInAction.action.started -= OnInPressed;
            inputActions.zoomInAction.action.canceled -= OnInReleased;

            inputActions.zoomOutAction.action.started -= OnOutPressed;
            inputActions.zoomOutAction.action.canceled -= OnOutReleased;
        }

        private void Update()
        {
            if (!IsSelected) return;

            if (_moveInput.magnitude > 0)
            {
                DoMove(_moveInput);
            }

            if (_outPressed)
            {
                distanceToObject += (zoomSpeed * Time.deltaTime);
                transform.position -= transform.forward * (zoomSpeed * Time.deltaTime);
            }

            if (_inPressed)
            {
                var newDistance = distanceToObject - zoomSpeed * Time.deltaTime;
                if (newDistance <= minDistanceToObject) return;
                distanceToObject = newDistance;
                transform.position += transform.forward * (zoomSpeed * Time.deltaTime);
            }
        }

        public void SetInitialPos()
        {
            var dir = (transform.position - currentLookAt).normalized;
            transform.position = currentLookAt + dir * distanceToObject;
            transform.LookAt(currentLookAt);
        }

        private void DoMove(Vector2 input)
        {
            Vector3 movement = (transform.up * input.y + transform.right * input.x).normalized *
                               (moveSpeed * Time.deltaTime);
            //move cam and current lookpoint by input
            Vector3 newLookAt = currentLookAt + movement;
            Vector3 newPos = transform.position + movement;

            //search new closest point
            var closestPoint = target.ClosestPoint(newPos);

            //make camera look at closest point
            transform.LookAt(closestPoint);

            //correct distance of camera
            var currDistance = (closestPoint - newPos).magnitude;
            var diff = currDistance - distanceToObject;
            newPos += transform.forward * diff;

            // clip distance traveled to movement vectors length for smooth movement
            var camClipping = (newPos - transform.position).normalized * movement.magnitude;
            var lookAtClipping = (newLookAt - currentLookAt).normalized * movement.magnitude;

            //set final currentTarget and pos after clipping (not look dir tho for some important reason!)
            transform.position += camClipping;
            currentLookAt += lookAtClipping;
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
            _inPressed = false;
            _outPressed = false;
        }

        private void OnMove(InputAction.CallbackContext obj)
        {
            _moveInput = obj.ReadValue<Vector2>();
        }

        private void OnInPressed(InputAction.CallbackContext obj)
        {
            _inPressed = true;
        }

        private void OnInReleased(InputAction.CallbackContext obj)
        {
            _inPressed = false;
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