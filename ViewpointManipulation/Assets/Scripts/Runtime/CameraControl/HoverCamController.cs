using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using Runtime.View.Manager;
using Runtime.View.ViewPair;
using Unity.VisualScripting;
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

    public enum UpMode
    {
        Global = 0, //uses the global up for the movement
        RestrictedGlobal = 1 << 0, //doesnt allow closer movement to north or south pole
        Local = 1 << 1, //uses the cameras up for the movement

        RectifyingLocal =
            1 << 2, //uses the cameras up for the movement but aligns its up to the global again after movement input is gone

        SelectiveLocal =
            1 << 3, //global but switches to local if close enough to north/south pole, also rectifies on movement end again
    }

    // based on https://dl.acm.org/doi/pdf/10.1145/1053427.1053439
    public class HoverCamController : MonoBehaviour
    {
        public float moveSpeed = 25f;
        public float zoomSpeed = 5f;

        public HoverActions inputActions;

        public float distanceToObject = 1f;
        public float minDistanceToObject = .5f;

        public UpMode upMode = UpMode.RestrictedGlobal;

        [ShowIf("upMode", UpMode.RestrictedGlobal)]
        public float restrictedGlobalAngle = 5f;

        [ShowIf("upMode", UpMode.SelectiveLocal)]
        public float selectiveLocalAngle = 5f;

        public Vector3 currentLookAt;

        public Outline modelOutline;

        [Foldout("Target-Outline Settings")]
        public bool shouldDisplayTargetOutline = true;

        [Foldout("Target-Outline Settings")]
        public Outline.Mode outlineMode = Outline.Mode.OutlineAll;

        [Foldout("Target-Outline Settings")]
        public Color outlineColor = Color.cyan;

        [Foldout("Target-Outline Settings")]
        public float outlineWidth = 2f;

        [HideInInspector]
        public HoverViewPair viewPair;

        [SerializeField]
        private bool _isSelected = false;

        private LocomotionSystem _locomotionSystem;

        private List<ActionBasedControllerManager> _locomotionControllerManagers =
            new List<ActionBasedControllerManager>();

        private Outline _targetOutlineInstance;
        private Vector3 _recentClosestPoint = Vector3.zero;
        private bool _temporarilyLocalUp = false;


        public Collider Target
        {
            get { return _target; }
            set
            {
                if (value == _target) return;
                _target = value;
                //a new target has been set, update the target outline if need be
                if (shouldDisplayTargetOutline)
                {
                    Destroy(_targetOutlineInstance);
                    _targetOutlineInstance = _target.AddComponent<Outline>();
                    _targetOutlineInstance.OutlineMode = outlineMode;
                    _targetOutlineInstance.OutlineColor = outlineColor;
                    _targetOutlineInstance.OutlineWidth = outlineWidth;
                }
            }
        }

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
                if (_targetOutlineInstance) _targetOutlineInstance.enabled = _isSelected;
                modelOutline.enabled = _isSelected;
                //update panels selection button to display correctly
                viewPair.basePanel.selectText.text = _isSelected ? "Unselect" : "Select";
                //disable/enable default vr locomotion actions
                SetLocomotionEnabled(!_isSelected);
                if (_isSelected)
                {
                    //enable actions
                    inputActions.EnableAllActions();
                    onSelected.Invoke(this);
                }
                else
                {
                    //disable actions
                    inputActions.DisableAllActions();
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
        private Collider _target;

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
            else if (upMode == UpMode.RectifyingLocal || upMode == UpMode.SelectiveLocal)
            {
                //make sure to rectify the cameras orientation so up is the global up again
                transform.LookAt(_recentClosestPoint);
                _temporarilyLocalUp = false;
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
            var closestPoint = _target.ClosestPoint(newPos);
            var angle = Vector3.Angle(_target.transform.up, closestPoint - newPos);

            if (upMode == UpMode.RestrictedGlobal)
            {
                //check if we would be in the restricted area
                if (angle <= restrictedGlobalAngle || (180 - angle) <= restrictedGlobalAngle) return;
            }
            else if (upMode == UpMode.SelectiveLocal)
            {
                //if we are in the selectiveLocalAngle we temporarily change to local up mode
                if (angle <= selectiveLocalAngle || (180 - angle) <= selectiveLocalAngle)
                    _temporarilyLocalUp = true;
            }

            _recentClosestPoint = closestPoint;

            //make camera look at closest point
            if (upMode == UpMode.Global || upMode == UpMode.RestrictedGlobal || !_temporarilyLocalUp)
            {
                transform.LookAt(closestPoint);
            }
            else if (upMode == UpMode.Local || upMode == UpMode.RectifyingLocal || _temporarilyLocalUp)
            {
                transform.LookAt(closestPoint,
                    transform.up); //this uses the local up so the camera can smoothly transition over the "north-pole" of the lookAtTarget
            }

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