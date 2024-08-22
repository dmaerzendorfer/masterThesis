using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Runtime.CameraControl
{
    // based on https://dl.acm.org/doi/pdf/10.1145/1053427.1053439
    public class HoverCamController : MonoBehaviour
    {
        public float moveSpeed = 25f;
        public float zoomSpeed = 5f;

        public InputActionReference moveAction;
        public InputActionReference zoomInAction;
        public InputActionReference zoomOutAction;

        public float distanceToObject = 1f;
        public float minDistanceToObject = .5f;

        public Collider target;
        public Vector3 currentLookAt;

        private bool _inPressed = false;
        private bool _outPressed = false;
        private Vector2 _moveInput = Vector2.zero;

        private void Start()
        {
            moveAction.action.performed += OnMove;

            zoomInAction.action.started += OnInPressed;
            zoomInAction.action.canceled += OnInReleased;

            zoomOutAction.action.started += OnOutPressed;
            zoomOutAction.action.canceled += OnOutReleased;


            //initial positioning of the hoverCam
            var dir = (transform.position - currentLookAt).normalized;
            transform.position = currentLookAt + dir * distanceToObject;
            transform.LookAt(currentLookAt);
        }

        private void OnDestroy()
        {
            moveAction.action.performed -= OnMove;
            zoomInAction.action.started -= OnInPressed;
            zoomInAction.action.canceled -= OnInReleased;

            zoomOutAction.action.started -= OnOutPressed;
            zoomOutAction.action.canceled -= OnOutReleased;
        }

        private void Update()
        {
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

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(currentLookAt, .05f);
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
            Debug.DrawLine(newPos, closestPoint);


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