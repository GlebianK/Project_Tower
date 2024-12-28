using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using UnityEngine.Windows;

[Serializable]
public class CharacterMovementEvent : UnityEvent { }

[Flags]
public enum PlayerMovementState
{
    GroundedFlag = 0x001,
    CrouchedFlag = 0x002,
    SprintingFlag = 0x004,
    DashFlag = 0x008,

    InAir = 0,
    Walk = 0x001,
    Crouch = 0x003,
    Sprint = 0x005,
    Slide = 0x007,
    Dash = 0x008,
}

[RequireComponent(typeof(CharacterController))]
public class PlayerMovementController : MonoBehaviour
{
    private CharacterController cc;

    [Header("General")]
    [SerializeField]
    private Transform _playerCamera;
    [SerializeField]
    private MovementInputEventHandler _movementInputEventHandler;
    [Header("Walking")]
    [SerializeField]
    private float _walkSpeed = 8.0F;
    [SerializeField]
    private float _acceleration = 15.0F;
    [SerializeField]
    private float _sprintSpeed = 12.0F;
    [SerializeField]
    private LayerMask _groundLayer;
    [Header("Airborne/Jump")]
    [SerializeField]
    private float _gravityForce = 9.8f;
    [SerializeField]
    private float _jumpVelocity = 7.7F;
    [SerializeField]
    private float _jumpVelocitySprinting = 6.77F;
    [SerializeField]
    private float _airMaxSpeed = 12f;
    [SerializeField]
    private float _wallJumpVerticalVelocity = 8.0f;
    [SerializeField]
    private float _wallJumpClimbingHorizontalImpulse = 3.0f;
    [SerializeField]
    private float _wallJumpHorizontalImpulse = 6.0f;
    [SerializeField]
    private int _wallJumpMaxCount = 3;

    public CharacterMovementEvent OnLanded;
    [Header("Crouch")]
    [SerializeField]
    private float _crouchSpeed = 0.3f;
    [SerializeField]
    private float _crouchHeight = 1.0F;

    [Header("Slide")]
    [SerializeField]
    private float _slideSpeed = 8.0f;
    [SerializeField]
    private float _slideDeceleration = 1.0f;
    [SerializeField]
    private float _slideMaxTime = 1.0f;
    [SerializeField]
    private float _slideStrafeCoeff = 0.1F;
    [SerializeField]
    private float _slideToCrouchSpeedThreshold = 0.3f;
    
    private float _uncrouchHeight = 0.0f;
    private float _slideTime = 0.0f;

    public CharacterMovementEvent OnSlideStarted;
    public CharacterMovementEvent OnSlideEnded;

    [Header("Dash")]
    [SerializeField]
    private float _dashRange = 15.0f;
    [SerializeField]
    private float _dashTime = 0.3f;


    public CharacterMovementEvent OnDashStarted;
    public CharacterMovementEvent OnDashEnded;

    [Header("Climbing")]
    [SerializeField]
    private ClimbChecker _climbChecker;
    public CharacterMovementEvent OnClimbStarted;
    public CharacterMovementEvent OnClimbEnded;
    public CharacterMovementEvent OnClimbHandDetach;

    private float DashSpeed => _dashTime > 0 ? _dashRange / _dashTime : 0;
    public Vector2 LookRotation => _lookRot;
    public Vector3 GroundNormal { get; private set; }
    public Vector3 CharacterVelocity => _characterVelocity;
    public float MaxSpeed => _walkSpeed;
    public float JumpVelocity => _jumpVelocity;

    private PlayerMovementState _moveState;
    public PlayerMovementState State => _moveState;
    public bool IsGrounded => _moveState.HasFlag(PlayerMovementState.GroundedFlag);


    private float _dashTimer = 0;
    private Vector3 _dashVector;

    private Vector3 _characterVelocity;

    private Vector3 _wsSlideVector = Vector3.zero;

    private float _airDelimitSpeed;
    private float _lastTimeJump;
    private float _targetHeight;
    private Vector3 _lastLandImpactSpeed;
    private int _wallJumpCounter;

    private Vector2 _lookRot = Vector2.zero;

    private bool _isTouchingWallInAir = false;
    private Vector3 _touchWallInAirNormal = Vector3.zero;

    public bool EnableMovement
    {
        get
        {
            return cc.enabled;
        }
        set
        {
            cc.enabled = value;
        }
    }

    private void Awake()
    {
        cc = GetComponent<CharacterController>();
    }

    private void Start()
    {
        cc.enableOverlapRecovery = true;
        _uncrouchHeight = cc.height;
        _targetHeight = cc.height;
        _moveState = PlayerMovementState.InAir;
        SetCrouchState(false, true);
        UpdateHeight();
        _lastTimeJump = -1;
    }

    private void Update()
    {
        if (EnableMovement)
            PerformMovement(Time.deltaTime);
    }

    public void OnSprintButtonAction(InputAction.CallbackContext context)
    {
        if (_moveState.HasFlag(PlayerMovementState.Dash)
            || !EnableMovement)
            return;

        bool pressed = context.started;

        if (pressed)
        {
            if (_moveState.Equals(PlayerMovementState.Sprint))
                _moveState &= ~PlayerMovementState.SprintingFlag;
            else if (_moveState.Equals(PlayerMovementState.Walk))
            {
                if (IsSprintAvailable())
                    _moveState |= PlayerMovementState.SprintingFlag;
                else if (_movementInputEventHandler.GetMovementDirectionRaw().sqrMagnitude > 0.2)
                {
                    IntoDash();
                }
            }
        }
    }

    public void OnJumpAction(InputAction.CallbackContext context)
    {
        if (_moveState.HasFlag(PlayerMovementState.Dash)
            || !EnableMovement)
            return;

        if (context.started)
        {
            if (_moveState.HasFlag(PlayerMovementState.GroundedFlag))
            {
                if (!_moveState.HasFlag(PlayerMovementState.CrouchedFlag))
                {
                    ProcessJump(false);
                }
            }
            else
            {
                if (_isTouchingWallInAir)
                {
                    if (Physics.CapsuleCast(
                            transform.position + Vector3.up * cc.radius,
                            transform.position + Vector3.up * (cc.height - cc.radius),
                            cc.radius,
                            -_touchWallInAirNormal,
                            out RaycastHit wallHit,
                            0.1f,
                            _groundLayer,
                            QueryTriggerInteraction.Ignore))
                    {

                        Vector3 wallHitNormal = wallHit.normal;
                        if (_wallJumpCounter < _wallJumpMaxCount)
                        {
                            _characterVelocity = Vector3.zero;
                            ProcessJump(true);

                            Vector3 lookDirection = transform.forward;
                            float lookNnormalDot = Vector3.Dot(wallHitNormal, lookDirection);
                            lookNnormalDot = Mathf.Clamp(lookNnormalDot, 0, 1);

                            float wallJumpActualHorizontalImpulse = Mathf.Lerp(_wallJumpClimbingHorizontalImpulse, _wallJumpHorizontalImpulse, lookNnormalDot);

                            _characterVelocity += Vector3.ProjectOnPlane(wallHitNormal, Vector3.up).normalized * wallJumpActualHorizontalImpulse;
                            _wallJumpCounter++;
                        }
                    }
                    else
                    {
                        _isTouchingWallInAir = false;
                        _touchWallInAirNormal = Vector3.zero;
                    }
                }
            }
                
        }
    }

    public void OnCrouchButtonAction(InputAction.CallbackContext context)
    {
        if (_moveState.Equals(PlayerMovementState.Dash) 
            || !_moveState.HasFlag(PlayerMovementState.GroundedFlag))
            return;
        
        if (context.started)
        {
            _moveState |= PlayerMovementState.CrouchedFlag;
            if (_moveState.Equals(PlayerMovementState.Slide))
            {
                IntoSlide();
            }
            else
            {
                SetCrouchState(true, false);
            }
        }
        else if (context.canceled)
        {
            if (_moveState.Equals(PlayerMovementState.Sprint))
            {
                IntoWalk();
            }
            else
            {
                SetCrouchState(false, false);
            }
        }
    }
    

    private void PerformMovement(float deltaTime)
    {
        HandleMovement(deltaTime);
    }

    private bool IsSprintAvailable()
    {
        return _moveState.HasFlag(PlayerMovementState.GroundedFlag)
            && !_moveState.HasFlag(PlayerMovementState.CrouchedFlag)
            && _movementInputEventHandler.GetMovementDirectionRaw().y > 0.7;
    }
    private void IntoWalk()
    {
        if (!cc.isGrounded)
        {
            _wallJumpCounter = 0;
            OnLanded?.Invoke();
        }

        _isTouchingWallInAir = false;
        _touchWallInAirNormal = Vector3.zero;
    }



    private void GroundCheck()
    {
        float chosenGroundCheckDistance = cc.skinWidth * 2 + cc.stepOffset;
        if (Time.time - _lastTimeJump > 0.2f)
        {
            Vector3 p1 = transform.position + Vector3.up * cc.radius;
            Vector3 p2 = transform.position + Vector3.up * (cc.height - cc.radius);
            if (Physics.CapsuleCast(p1, 
                p2,
                cc.radius, Vector3.down, out RaycastHit hit, chosenGroundCheckDistance, 
                _groundLayer,
                QueryTriggerInteraction.Ignore))
            {
                GroundNormal = hit.normal;
                if (Vector3.Dot(GroundNormal, Vector3.up) > 0 &&
                    IsNormalUnderSlope(GroundNormal))
                {
                    _moveState |= PlayerMovementState.GroundedFlag;
                    if (hit.distance > cc.skinWidth)
                        cc.Move(Vector3.down * hit.distance);
                }
            }
            else
            {
                
                _moveState &= ~PlayerMovementState.GroundedFlag;
                GroundNormal = Vector3.up;
            }
        }
    }

    private void PerformWalking(float deltaTime)
    {
        Vector3 movementInput = _movementInputEventHandler.GetMovementDirectionRaw();
        Vector3 WSmovementInput = _movementInputEventHandler.GetMovementDirectionInTransformSpace(transform);
        
        if (_moveState.HasFlag(PlayerMovementState.SprintingFlag) && 
            !IsSprintAvailable()) 
        {
            _moveState &= ~PlayerMovementState.SprintingFlag;
        }

        if (!_moveState.HasFlag(PlayerMovementState.CrouchedFlag))
        {
            SetCrouchState(false, false);
        }

        _isTouchingWallInAir = false;
        float speed = _moveState.Equals(PlayerMovementState.Crouch) ? _crouchSpeed 
            : _moveState.Equals(PlayerMovementState.Sprint) ? _sprintSpeed : _walkSpeed;
        Vector3 targetVelocity = WSmovementInput * speed;
        targetVelocity = GetDirectionOnSlope(targetVelocity.normalized, GroundNormal) * targetVelocity.magnitude;
        _characterVelocity = Vector3.Lerp(_characterVelocity, targetVelocity, _acceleration * deltaTime);
    }

    private void IntoAir()
    {
        Vector3 horizontalVelocity = CharacterVelocity;
        horizontalVelocity.y = 0;
        _airDelimitSpeed = horizontalVelocity.magnitude > _airMaxSpeed ? horizontalVelocity.magnitude : _airMaxSpeed;
        _moveState &= ~PlayerMovementState.GroundedFlag;
        GroundNormal = Vector3.up;

        _isTouchingWallInAir = false;
        _touchWallInAirNormal = Vector3.zero;
        _wallJumpCounter = 0;
        if (_moveState.HasFlag(PlayerMovementState.CrouchedFlag))
        {
            SetCrouchState(false, false);
        }
    }
    private void PerformInAir(float deltaTime)
    {
        
        Vector3 movementInput = _movementInputEventHandler.GetMovementDirectionRaw();
        Vector3 WSmovementInput = _movementInputEventHandler.GetMovementDirectionInTransformSpace(transform);

        _characterVelocity += WSmovementInput * _acceleration * deltaTime;
        float VertVelocity = _characterVelocity.y;
        Vector3 HorizontalVelocity = Vector3.ProjectOnPlane(CharacterVelocity, Vector3.up);

        float gravityScale = 1;
        HorizontalVelocity = Vector3.ClampMagnitude(HorizontalVelocity, _airDelimitSpeed);
        _characterVelocity = HorizontalVelocity + Vector3.up * VertVelocity;

        _characterVelocity += Vector3.down * _gravityForce * deltaTime * gravityScale;
    }

    private void IntoSlide()
    {
        Vector3 WSmovementInput = _movementInputEventHandler.GetMovementDirectionInTransformSpace(transform);
        _slideTime = 0;
        if (SetCrouchState(true, false))
        {
            _wsSlideVector = WSmovementInput.magnitude == 0 ? transform.forward : WSmovementInput;
        }
        else
        {
            _moveState = PlayerMovementState.Crouch;
            IntoWalk();
        }
    }
    private void PerformSliding(float deltaTime)
    {
        Vector3 wsInput = (_wsSlideVector + Vector3.Cross(_wsSlideVector, Vector3.up) * _slideStrafeCoeff).normalized;

        Vector3 targetVelocity = wsInput * (_slideSpeed - _slideDeceleration * _slideTime);
        targetVelocity = GetDirectionOnSlope(targetVelocity.normalized, GroundNormal) * targetVelocity.magnitude;
        _characterVelocity = Vector3.Lerp(_characterVelocity, targetVelocity, _acceleration * deltaTime);

        _slideTime += deltaTime;

        if (_slideTime > _slideMaxTime)
        {
            _moveState &= ~PlayerMovementState.Sprint;
            IntoWalk();
        }
    }
    private void IntoDash()
    {
        Vector3 movementInput = _movementInputEventHandler.GetMovementDirectionRaw();
        Vector3 WSmovementInput = _movementInputEventHandler.GetMovementDirectionInTransformSpace(transform);

        _dashVector = WSmovementInput.magnitude > 0 ? WSmovementInput : transform.forward;
        _dashTimer = 0;
        _moveState = PlayerMovementState.Dash;
        OnDashStarted?.Invoke();
    }
    private void PerformDash(float deltaTime)
    {
        _characterVelocity = _dashVector.normalized * DashSpeed;

        _dashTimer += deltaTime;
        if (_dashTimer >= _dashTime)
        {
            _moveState &= ~PlayerMovementState.DashFlag;
            _dashTimer = 0;
            IntoAir();
            OnDashEnded?.Invoke();
            _characterVelocity = Vector3.ClampMagnitude(_characterVelocity, _walkSpeed);
        }
    }

    private bool CanUncrouch()
    {
        Collider[] standingOverlaps = Physics.OverlapCapsule(
                    transform.position + Vector3.up * cc.radius,
                    transform.position + Vector3.up * (_uncrouchHeight - cc.radius),
                    cc.radius,
                    -1,
                    QueryTriggerInteraction.Ignore);

        foreach (Collider c in standingOverlaps)
        {
            if (c != cc) return false;
        }
        return true;
    }

    private IEnumerator TrySetUncrouchForWhile()
    {
        while (!CanUncrouch())
        {
            yield return null;
        }

        SetCrouchState(false, false);
    }

    Coroutine _uncrouchCoroutine = null;

    private bool SetCrouchState(bool crouch, bool ignoreObstructions)
    {
        if (crouch)
        {
            if (_uncrouchCoroutine != null)
                StopCoroutine(_uncrouchCoroutine);
            _targetHeight = _crouchHeight;
        }
        else
        {
            if (!ignoreObstructions)
            {
                if (!CanUncrouch())
                {
                    if (_uncrouchCoroutine != null)
                        StopCoroutine(_uncrouchCoroutine);
                    _uncrouchCoroutine = StartCoroutine(TrySetUncrouchForWhile());
                    return false;
                }
            }
            _targetHeight = _uncrouchHeight;
        }
        _moveState = crouch ? _moveState | PlayerMovementState.CrouchedFlag
            : _moveState & ~PlayerMovementState.CrouchedFlag;
        UpdateHeight();
        return true;
    }

    private void UpdateHeight()
    {
        cc.height = _targetHeight;
        cc.center = Vector3.up * cc.height * 0.5f;
        _playerCamera.transform.localPosition = Vector3.up * cc.height * 0.8f;
    }

    private void HandleMovement(float deltaTime)
    {

        if (_moveState.Equals(PlayerMovementState.Dash))
        {
            PerformDash(deltaTime);
        }
        else
        {
            bool wasGrounded = IsGrounded;

            GroundNormal = Vector3.up;
            GroundCheck();
            bool nowGrounded = IsGrounded;

            Vector3 WSmovementInput = _movementInputEventHandler.GetMovementDirectionInTransformSpace(transform);

            if (nowGrounded && !wasGrounded)
            {
                IntoWalk();
            }
            if (!nowGrounded && wasGrounded)
            {
                IntoAir();
            }
            if (_moveState.Equals(PlayerMovementState.Slide))
            {
                PerformSliding(deltaTime);
            }
            else if (IsGrounded)
            {
                PerformWalking(deltaTime);
            }
            else
            {
                if (!_moveState.Equals(PlayerMovementState.InAir))
                    IntoAir();
                PerformInAir(deltaTime);
            }
        }
        Vector3 capsuleBottomBeforeMove = transform.position + transform.up * cc.radius;
        Vector3 capsuleTopBeforeMove = transform.position + transform.up * (cc.height - cc.radius);
        bool wasSliding = _moveState.Equals(PlayerMovementState.Slide);

        Vector3 moveVec = CharacterVelocity * deltaTime;
        if (IsGrounded)
            moveVec += Vector3.down * 0.05f;
        cc.Move(moveVec);
        if (Physics.CapsuleCast(
                capsuleBottomBeforeMove,
                capsuleTopBeforeMove,
                cc.radius,
                CharacterVelocity.normalized,
                out RaycastHit hit,
                CharacterVelocity.magnitude * deltaTime,
                -1, QueryTriggerInteraction.Ignore))
        {
            _lastLandImpactSpeed = cc.velocity * deltaTime;
            _characterVelocity = Vector3.ProjectOnPlane(CharacterVelocity, hit.normal);

            if (wasSliding && (hit.distance < _slideToCrouchSpeedThreshold * deltaTime))
            {
                _moveState &= ~PlayerMovementState.Sprint;
                IntoWalk();
            }
            if (!cc.isGrounded && !IsNormalUnderSlope(hit.normal))
            {
                if (Vector3.Dot(transform.up, hit.normal) >= -0.2f)
                _isTouchingWallInAir = true;
                _touchWallInAirNormal = hit.normal;
                _characterVelocity.x *= 1 - deltaTime;
                _characterVelocity.z *= 1 - deltaTime;

                if (_climbChecker)
                {
                    RaycastHit climbHit = _climbChecker.GetEndClimbPoint(cc);
                    if (climbHit.collider != null)
                    {
                        StartCoroutine(_climbChecker.ClimbOverTime(this, climbHit, hit.normal));
                        _characterVelocity = Vector3.zero;
                    }
                }
            } 
        }

        
    }
    private Vector3 GetDirectionOnSlope(Vector3 direction, Vector3 slopeNormal)
    {
        Vector3 directionRight = Vector3.Cross(direction, Vector3.up);
        return Vector3.Cross(slopeNormal, directionRight).normalized;
    }
    private bool IsNormalUnderSlope(Vector3 normal)
    {
        float angle = Vector3.Angle(transform.up, normal);
        return angle <= cc.slopeLimit;
    }

    private void ProcessJump(bool isWallJump)
    {
        if (SetCrouchState(false, false))
        {
            IntoAir();
            _characterVelocity.y = 0;
            float jumpVelocity = _moveState.HasFlag(PlayerMovementState.SprintingFlag)
                ? _jumpVelocitySprinting
                : _jumpVelocity;
            jumpVelocity = isWallJump 
                ? _wallJumpVerticalVelocity
                : jumpVelocity;

            _characterVelocity += Vector3.up * jumpVelocity;
            _lastTimeJump = Time.time;
        }
        
    }


}
