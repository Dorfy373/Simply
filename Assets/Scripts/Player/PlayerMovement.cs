using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{

    [Header("Jump Animation")]
    [SerializeField] private float _fallVelocityThreshold = 4f;
    [SerializeField] private float _fallGraceTime = 0.5f;

    [Header("Dash / Sprint Button")]
    [SerializeField] private float _tapThreshold = 0.2f;

    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _sprintSpeed = 10f;
    [SerializeField] private float _jumpHeight = 1.5f;
    [SerializeField] private float _gravity = -9.81f;
    [SerializeField] private float _dashSpeed = 15f;
    [SerializeField] private float _dashDuration = 0.2f;
    [SerializeField] private float _dashCooldown = 1f;

    [Header("Rotation")]
    [SerializeField] private float _rotationSmoothness = 0.15f;

    [Header("Slope Handling")]
    [SerializeField] private float _slopeSlideSpeed = 8f;


    [Header("Components")]
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private PlayerAnimator _playerAnimator;
    [SerializeField] private PlayerHealth _playerHealth;
    [SerializeField] private PlayerEnergy _playerEnergy;
    [SerializeField] private PlayerHUDController _hud;
    private CharacterController _characterController;
    private Camera _mainCamera;

    [Header("Other")]
    private float _airborneTimer;
    private float _currentSpeed;
    private Vector2 _moveInput;
    private Vector3 _velocity;
    private bool _jumpPressed;
    private bool _isSprinting;
    private bool _wasGrounded;
    private bool _isFalling;
    private bool _isAttacking;
    private bool _isDashing;
    private bool _sprintThresholdReached;
    private float _dashTimeRemaining;
    private float _lastDashTime = -Mathf.Infinity;
    private Vector3 _dashDirection;
    private Coroutine _sprintHoldRoutine;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _wasGrounded = _characterController.isGrounded;
    }

    private void OnEnable()
    {
        _inputReader.MoveEvent += HandleMove;
        _inputReader.SprintPerformed += HandleSprintPerformed;
        _inputReader.SprintCanceled += HandleSprintCanceled;
        _inputReader.JumpPerformed += HandleJumpPerformed;
        _inputReader.JumpCanceled += HandleJumpCanceled;
    }

    private void Start()
    {
        _mainCamera = Camera.main;

    }

    private void Update()
    {
        UpdatePlayer();
    }

    private void UpdatePlayer()
    {

        HandleMovementSpeed();
        HandleRotation();
        HandleGravityAndJump();
        HandleGroundedStateChange();
        HandleDash();
        HandleEnergySpending();
        Move();
        
    }

    private void HandleMove(Vector2 input) => _moveInput = input;
    private void HandleJumpPerformed() => _jumpPressed = true;
    private void HandleJumpCanceled() => _jumpPressed = false;

    private void Move()
    {
        if (IsOnSteepSlope(out Vector3 slopeNormal))
        {
            Vector3 slideDirection = Vector3.ProjectOnPlane(Vector3.down, slopeNormal).normalized;
            Vector3 slideVelocity = slideDirection * _slopeSlideSpeed;
            slideVelocity.y += _velocity.y;

            _characterController.Move(slideVelocity * Time.deltaTime);
            _playerAnimator.MovementAnimation(false, false);
            return;
        }

        Vector3 horizontalMove;
        if (_isDashing)
        {
            horizontalMove = _dashDirection * _dashSpeed;
        }
        else
        {
            horizontalMove = _isAttacking ? Vector3.zero : GetCameraDirection() * _currentSpeed;
        }

        Vector3 finalMove = horizontalMove;
        finalMove.y = _velocity.y;

        _characterController.Move(finalMove * Time.deltaTime);

        bool isMoving = !_isAttacking && !_isDashing && horizontalMove.magnitude > 0.1f;
        _playerAnimator.MovementAnimation(isMoving, _isSprinting);
    }
    private void HandleRotation()
    {
        if (_mainCamera == null || _moveInput == Vector2.zero)
            return;

        Vector3 direction = GetCameraDirection();

        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSmoothness);
        }
    }

    private void HandleMovementSpeed()
    {
        _currentSpeed = _isSprinting ? _sprintSpeed : _moveSpeed;
    }

    private Vector3 GetCameraDirection()
    {
        if (_mainCamera == null) return Vector3.zero;

        Vector3 cameraForward = _mainCamera.transform.forward;
        Vector3 cameraRight = _mainCamera.transform.right;

        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();

        return cameraForward * _moveInput.y + cameraRight * _moveInput.x;
    }

    private void HandleSprintPerformed()
    {
        _sprintHoldRoutine = StartCoroutine(SprintHoldRoutine());
    }
    private void HandleSprintCanceled()
    {
        if(_sprintHoldRoutine != null)
        {
            StopCoroutine(_sprintHoldRoutine);
            _sprintHoldRoutine = null;
        }
        if (_sprintThresholdReached)
        {
            _isSprinting = false;
            _sprintThresholdReached = false;
            _hud?.SetRunPressed(false);
        }
        else
        {
            TryDash();
        }
    }

    private void HandleGravityAndJump()
    {
        if (_characterController.isGrounded && _velocity.y < 0)
            _velocity.y = -2f;

        if (_jumpPressed && _characterController.isGrounded)
        {
            _velocity.y = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
            _jumpPressed = false;
            _playerAnimator.TriggerJumpUp();
        }

        _velocity.y += _gravity * Time.deltaTime;
    }

    private void HandleGroundedStateChange()
    {
        bool isGroundedNow = _characterController.isGrounded;

        if (!isGroundedNow)
        {
            _airborneTimer += Time.deltaTime;
        }
        else
        {
            _airborneTimer = 0f;
        }

        bool shouldBeFalling = _airborneTimer >= _fallGraceTime && _velocity.y < _fallVelocityThreshold;

        if (shouldBeFalling != _isFalling)
        {
            _isFalling = shouldBeFalling;
            _playerAnimator.TriggerFalling(_isFalling);
        }

        if (!_wasGrounded && isGroundedNow) 
        {
            _playerAnimator.TriggerLand();
            _isFalling = false;
            _playerAnimator.TriggerFalling(_isFalling);
            _airborneTimer = 0f;
        }

        _wasGrounded = isGroundedNow;
    }
    private void TryDash()
    {
        if (_isDashing || Time.time < _lastDashTime + _dashCooldown || _isAttacking || !_playerEnergy.HasEnergy) return;

        _isDashing = true;
        _dashTimeRemaining = _dashDuration;
        _lastDashTime = Time.time;

        _dashDirection = _moveInput != Vector2.zero
            ? GetCameraDirection().normalized
            : transform.forward;

        _playerHealth.SetInvulnerable(true);
        _playerAnimator.TriggerDash();

        _hud?.SetDashPressed(true);
        StartCoroutine(ReleaseDashButtonAfterDelay(_dashDuration));

    }

    private void HandleDash()
    {
        if (!_isDashing) return;

        _dashTimeRemaining -= Time.deltaTime;

        if (_dashTimeRemaining <= 0f)
        {
            _isDashing = false;
            _playerHealth.SetInvulnerable(false);
        }
    }

    private void HandleEnergySpending()
    {
        if(_isSprinting && !_playerEnergy.HasEnergy)
        {
            _isSprinting = false;
        }

        bool isActuallyMoving = _moveInput != Vector2.zero;
        _playerEnergy.SetSpending((_isSprinting && isActuallyMoving) || _isDashing);
    }

    private bool IsOnSteepSlope(out Vector3 slopeNormal)
    {
        slopeNormal = Vector3.up;

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, _characterController.height / 2f + 0.3f))
        {
            slopeNormal = hit.normal;
            float angle = Vector3.Angle(hit.normal, Vector3.up);
            return angle > _characterController.slopeLimit;
        }

        return false;
    }

    public void SetAttacking(bool isAttacking)
    {
        _isAttacking = isAttacking;
    }
    private void OnDisable()
    {
        _inputReader.MoveEvent -= HandleMove;
        _inputReader.SprintPerformed -= HandleSprintPerformed;
        _inputReader.SprintCanceled -= HandleSprintCanceled;
        _inputReader.JumpPerformed -= HandleJumpPerformed;
        _inputReader.JumpCanceled -= HandleJumpCanceled;
    }
    private IEnumerator SprintHoldRoutine()
    {
        yield return new WaitForSeconds(_tapThreshold);
        _sprintThresholdReached = true;
        _isSprinting = true;

        _hud?.SetRunPressed(true);
    }
    private IEnumerator ReleaseDashButtonAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        _hud?.SetDashPressed(false);
    }
}
