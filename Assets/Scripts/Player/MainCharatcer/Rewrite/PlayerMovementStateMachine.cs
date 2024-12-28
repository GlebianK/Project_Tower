using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovementStateMachine : MonoBehaviour
{
    private CharacterController _cc;
    [SerializeField] 
    private MovementInputEventHandler _movementInputEventHandler;
    [SerializeField]
    private List<PlayerMovementStateConfigBase> _moveStatesList;

    [SerializeField]
    private string _startMovementStateName;
    [SerializeField]
    private LayerMask _groundLayer;

    private Dictionary<int, IPlayerMovementState> _movementStates
        = new Dictionary<int, IPlayerMovementState>();

    private IPlayerMovementState _currentMovementState;

    //additional vals for good machine management
    public CharacterController CC => _cc;
    public Vector3 GroundNormal { get; private set; }
    public Vector3 CharacterVelocity;

    private float _lastTimeJump;

    private void Awake()
    {
        _cc = GetComponent<CharacterController>();
        _lastTimeJump = -1;
    }

    private void Start()
    {
        foreach (var stateConfig in _moveStatesList)
        {
            int hash = stateConfig.Name.GetHashCode();
            IPlayerMovementState state = stateConfig.CreateMovementStateInstance(
                this,
                _movementInputEventHandler
                );
            _movementStates.Add(hash, state);
        }

        SetCurrentState(_startMovementStateName);
        CharacterVelocity = Vector3.zero;
        GroundNormal = Vector3.up;
    }

    private void Update()
    {
        _currentMovementState.UpdateMovementVelocity(Time.deltaTime);

        Vector3 capsuleBottomBeforeMove = transform.position + transform.up * _cc.radius;
        Vector3 capsuleTopBeforeMove = transform.position + transform.up * (_cc.height - _cc.radius);
        Vector3 moveVec = CharacterVelocity * Time.deltaTime;

        _cc.Move(moveVec);
        RaycastHit hitResult;
        if (Physics.CapsuleCast(
                capsuleBottomBeforeMove,
                capsuleTopBeforeMove,
                _cc.radius,
                CharacterVelocity.normalized,
                out hitResult,
                CharacterVelocity.magnitude * Time.deltaTime,
                -1, QueryTriggerInteraction.Ignore))
        {
            CharacterVelocity = Vector3.ProjectOnPlane(CharacterVelocity, hitResult.normal);
        }

        _currentMovementState.HandleObstacleAfterMovement(Time.deltaTime, hitResult);

        _movementInputEventHandler.jumpPressed = false;
    }

    //machine manage methods

    public IPlayerMovementState GetCurrentState()
    {
        return _currentMovementState;
    }
    public void SetCurrentState(string stateName)
    {
        SetCurrentState(stateName.GetHashCode());
    }
    public void SetCurrentState(int hash)
    {
        IPlayerMovementState newState = _movementStates[hash];
        if (newState == _currentMovementState)
            return;

        IPlayerMovementState oldState = _currentMovementState;
        _currentMovementState = newState;
        oldState?.OnStateDeactivated(newState);
        newState?.OnStateActivated(oldState);
    }

    private void UpdateHeight(float height)
    {
        _cc.height = height;
        _cc.center = Vector3.up * _cc.height * 0.5f;
    }

    Coroutine _uncrouchCoroutine = null;

    public bool CanSetHeight(float height)
    {
        Collider[] standingOverlaps = Physics.OverlapCapsule(
                    transform.position + Vector3.up * _cc.radius,
                    transform.position + Vector3.up * (height - _cc.radius),
                    _cc.radius,
                    -1,
                    QueryTriggerInteraction.Ignore);

        foreach (Collider c in standingOverlaps)
        {
            if (c != _cc) return false;
        }
        return true;
    }

    private IEnumerator TrySetHeightForWhile(float height)
    {
        while (!CanSetHeight(height))
        {
            yield return null;
        }

        UpdateHeight(height);
    }

    public void SetHeight(float newHeight, bool ignoreObstructions)
    {
        if (_cc.height > newHeight)
        {
            if (_uncrouchCoroutine != null)
                StopCoroutine(_uncrouchCoroutine);
            UpdateHeight(newHeight);
        }
        else
        {
            if (!ignoreObstructions)
            {
                if (!CanSetHeight(newHeight))
                {
                    if (_uncrouchCoroutine != null)
                        StopCoroutine(_uncrouchCoroutine);
                    _uncrouchCoroutine = StartCoroutine(TrySetHeightForWhile(newHeight));
                    return;
                }
            }
            UpdateHeight(newHeight);
        }
    }
    public bool GroundCheck()
    {
        float chosenGroundCheckDistance = _cc.skinWidth + _cc.stepOffset;
        GroundNormal = Vector3.up;
        bool isGrounded = false;
        float afterJumpTime = Time.time - _lastTimeJump;
        if (afterJumpTime < 0.12f)
        {
            return false;
        }
        Vector3 p1 = transform.position + Vector3.up * _cc.radius;
        Vector3 p2 = transform.position + Vector3.up * (_cc.height - _cc.radius);
        if (Physics.CapsuleCast(p1,
            p2,
            _cc.radius, Vector3.down, out RaycastHit hit, chosenGroundCheckDistance,
            _groundLayer,
            QueryTriggerInteraction.Ignore))
        {
            GroundNormal = hit.normal;
            isGrounded = true;
            if (Vector3.Dot(GroundNormal, Vector3.up) > 0 &&
                IsNormalUnderSlope(GroundNormal))
            {
                if (hit.distance > _cc.skinWidth)
                    _cc.Move(Vector3.down * hit.distance);
            }
        }
        return isGrounded;

    }

    //helpers
    public Vector3 GetDirectionOnSlope(Vector3 direction, Vector3 slopeNormal)
    {
        Vector3 directionRight = Vector3.Cross(direction, Vector3.up);
        return Vector3.Cross(slopeNormal, directionRight).normalized;
    }
    public bool IsNormalUnderSlope(Vector3 normal)
    {
        float angle = Vector3.Angle(transform.up, normal);
        return angle <= _cc.slopeLimit;
    }

    public void Jump(Vector3 jumpVelocity)
    {
        CharacterVelocity = jumpVelocity;
        _lastTimeJump = Time.time;
    }
}
