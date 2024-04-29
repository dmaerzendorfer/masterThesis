using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Generics.Scripts.Runtime
{
    //based on https://www.youtube.com/watch?v=WNV9l04s8t4

    [RequireComponent(typeof(CharacterController))]
    public class SimplePlayerController : MonoBehaviour
    {
        private const float Gravity = -9.81f;

        #region Animations

        [SerializeField]
        private Animator animator;

        //animation hashes
        private int _isJoggingHash;
        private int _isSprintingHash;
        private int _isGroundedHash;

        #endregion

        #region MovementVariables

        [SerializeField]
        private Movement movement;

        private CharacterController _characterController;
        private Vector2 _input;
        private Vector3 _direction;

        #endregion

        #region RotationVariables

        [SerializeField]
        private float smoothTime = 0.05f; //for rotation

        private float _currentRotationVelocity; //for rotation looking

        #endregion

        #region GravityVariables

        [SerializeField]
        private float gravityMultiplier = 1f;

        [SerializeField]
        private float coyoteTime = .1f;

        private float _currentCoyoteTime = 0f;

        private float _gravityVelocity; //for gravity

        #endregion

        #region JumpVariables

        [SerializeField]
        private float jumpPower = 3f;

        [SerializeField]
        private int maxNumberOfJumps = 2;

        private int _numberOfJumps;

        #endregion


        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();

            //setup animations
            _isJoggingHash = Animator.StringToHash("isJogging");
            _isSprintingHash = Animator.StringToHash("isSprinting");
            _isGroundedHash = Animator.StringToHash("isGrounded");
        }

        private void Update()
        {
            CoyoteControl();
            ApplyGravity();
            ApplyRotation();
            ApplyMovement();

            CoyoteControl();
        }

        private void ApplyGravity()
        {
            if (IsGrounded() && _gravityVelocity < 0.0f && gravityMultiplier != 0f)
            {
                animator.SetBool(_isGroundedHash, true);
                _gravityVelocity = -1.0f;
            }
            else
            {
                animator.SetBool(_isGroundedHash, false);
                _gravityVelocity += Gravity * gravityMultiplier * Time.deltaTime;
            }

            _direction.y = _gravityVelocity;
        }

        private void ApplyRotation()
        {
            if (_input.sqrMagnitude == 0) return;
            var targetAngle = Mathf.Atan2(_direction.x, _direction.z) * Mathf.Rad2Deg;
            var angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _currentRotationVelocity,
                smoothTime);
            transform.rotation = Quaternion.Euler(0.0f, angle, 0.0f);
        }

        private void ApplyMovement()
        {
            if (_input.sqrMagnitude == 0)
            {
                animator.SetBool(_isJoggingHash, false);
            }
            else
            {
                animator.SetBool(_isJoggingHash, true);
            }

            var targetSpeed = movement.isSprinting ? movement.speed * movement.multiplier : movement.speed;
            movement.currentSpeed =
                Mathf.MoveTowards(movement.currentSpeed, targetSpeed, movement.acceleration * Time.deltaTime);
            var unscaledJump = _direction.y;
            var motion = _direction * (movement.currentSpeed * Time.deltaTime);
            motion.y = unscaledJump * (movement.speed * Time.deltaTime);
            _characterController.Move(motion);
        }

        public void Move(InputAction.CallbackContext context)
        {
            _input = context.ReadValue<Vector2>();
            _direction = new Vector3(_input.x, 0.0f, _input.y);
        }

        public void Jump(InputAction.CallbackContext context)
        {
            if (!context.started) return;
            if (!IsGrounded() && _numberOfJumps >= maxNumberOfJumps) return;
            if (_numberOfJumps == 0) StartCoroutine(WaitForLanding());

            _numberOfJumps++;
            _gravityVelocity = jumpPower;
            // _gravityVelocity = jumpPower / _numberOfJumps;
        }

        public void Sprint(InputAction.CallbackContext context)
        {
            movement.isSprinting = context.started || context.performed;
            animator.SetBool(_isSprintingHash, movement.isSprinting);
        }

        private IEnumerator WaitForLanding()
        {
            yield return new WaitUntil(() => !IsGrounded());
            yield return new WaitUntil(IsGrounded);

            _numberOfJumps = 0;
        }

        private bool IsGrounded()
        {
            // return _characterController.isGrounded;
            return _currentCoyoteTime < coyoteTime;
        }

        public void CoyoteControl()
        {
            if (_characterController.isGrounded)
            {
                _currentCoyoteTime = 0f;
            }
            else
            {
                _currentCoyoteTime += Time.deltaTime;
            }
        }
    }
}

[Serializable]
public struct Movement
{
    public float speed;
    public float multiplier;
    public float acceleration;
    [HideInInspector] public bool isSprinting;
    [HideInInspector] public float currentSpeed;
}