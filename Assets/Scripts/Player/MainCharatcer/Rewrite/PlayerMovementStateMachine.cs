using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovementStateMachine : MonoBehaviour
{
    private CharacterController cc;
    [SerializeField] 
    private MovementInputEventHandler movementInputEventHandler;
    [SerializeField]
    private List<PlayerMovementStateConfigBase> moveStatesList;

    [SerializeField]
    private string startMovementStateName;
    [SerializeField]
    private LayerMask groundLayer;

    private Dictionary<int, IPlayerMovementState> movementStates;

    private IPlayerMovementState currentMovementState;

    //additional vals for good machine management
    public CharacterController CC => cc;
    public Vector3 GroundNormal { get; private set; }
    public Vector3 CharacterVelocity;

    private float lastTimeJump;

    public float PlayerDefaultHeight { get; private set; }
    
    private void Awake()
    {
        cc = GetComponent<CharacterController>();
        lastTimeJump = -1;

        movementStates = new Dictionary<int, IPlayerMovementState>();
    }

    private void Start()
    {
        foreach (var stateConfig in moveStatesList)
        {
            int hash = stateConfig.Name.GetHashCode();
            IPlayerMovementState state = stateConfig.CreateMovementStateInstance(
                this,
                movementInputEventHandler
                );
            movementStates.Add(hash, state);
        }

        SetCurrentState(startMovementStateName);
        CharacterVelocity = Vector3.zero;
        GroundNormal = Vector3.up;
        PlayerDefaultHeight = cc.height;
    }

    private void Update()
    {
        currentMovementState.UpdateMovementVelocity(Time.deltaTime);

        Vector3 capsuleBottomBeforeMove = transform.position + transform.up * cc.radius;
        Vector3 capsuleTopBeforeMove = transform.position + transform.up * (cc.height - cc.radius);
        Vector3 moveVec = CharacterVelocity * Time.deltaTime;

        cc.Move(moveVec);
        RaycastHit hitResult;
        if (Physics.CapsuleCast(
                capsuleBottomBeforeMove,
                capsuleTopBeforeMove,
                cc.radius,
                CharacterVelocity.normalized,
                out hitResult,
                CharacterVelocity.magnitude * Time.deltaTime,
                -1, QueryTriggerInteraction.Ignore))
        {
            CharacterVelocity = Vector3.ProjectOnPlane(CharacterVelocity, hitResult.normal);
        }

        currentMovementState.HandleObstacleAfterMovement(Time.deltaTime, hitResult);

        movementInputEventHandler.jumpPressed = false;
    }

    //machine manage methods

    public IPlayerMovementState GetCurrentState()
    {
        return currentMovementState;
    }

    public IPlayerMovementState GetStateByNameHash(int hash)
    {
        return movementStates[hash];
    }
    public IPlayerMovementState GetStateByName(string name)
    {
        return GetStateByNameHash(name.GetHashCode());
    }

    public void SetCurrentState(string stateName)
    {
        SetCurrentState(stateName.GetHashCode());
    }
    public void SetCurrentState(int hash)
    {
        IPlayerMovementState newState = movementStates[hash];
        if (newState == currentMovementState)
            return;

        IPlayerMovementState oldState = currentMovementState;
        currentMovementState = newState;
        oldState?.OnStateDeactivated(newState);
        newState?.OnStateActivated(oldState);
    }

    private void UpdateHeight(float height)
    {
        cc.height = height;
        cc.center = Vector3.up * cc.height * 0.5f;
    }

    Coroutine uncrouchCoroutine = null;

    public bool CanSetHeight(float height)
    {
        Collider[] standingOverlaps = Physics.OverlapCapsule(
                    transform.position + Vector3.up * cc.radius,
                    transform.position + Vector3.up * (height - cc.radius),
                    cc.radius,
                    -1,
                    QueryTriggerInteraction.Ignore);

        foreach (Collider c in standingOverlaps)
        {
            if (c != this.cc) return false;
        }
        return true;
    }

    private IEnumerator TrySetHeightForWhileCoroutine(float height)
    {
        while (!CanSetHeight(height))
        {
            yield return null;
        }

        UpdateHeight(height);
    }

    public void ResetHeight()
    {
        SetHeight(PlayerDefaultHeight, false);
    }

    public void SetHeight(float newHeight, bool ignoreObstructions)
    {
        if (cc.height > newHeight)
        {
            if (uncrouchCoroutine != null)
                StopCoroutine(uncrouchCoroutine);
            UpdateHeight(newHeight);
        }
        else
        {
            if (!ignoreObstructions)
            {
                if (!CanSetHeight(newHeight))
                {
                    if (uncrouchCoroutine != null)
                        StopCoroutine(uncrouchCoroutine);
                    uncrouchCoroutine = StartCoroutine(TrySetHeightForWhileCoroutine(newHeight));
                    return;
                }
            }
            UpdateHeight(newHeight);
        }
    }
    public bool GroundCheck()
    {
        float chosenGroundCheckDistance = cc.isGrounded ? cc.skinWidth + 1 : 0.2f;
        GroundNormal = Vector3.up;
        bool isGrounded = false;
        float afterJumpTime = Time.time - lastTimeJump;
        if (afterJumpTime < 0.12f)
        {
            return false;
        }
        //Vector3 p1 = transform.position + Vector3.up * cc.radius;
        //Vector3 p2 = transform.position + Vector3.up * (cc.height - cc.radius);
        //if (Physics.CapsuleCast(p1,
        //    p2,
        //    cc.radius, Vector3.down, out RaycastHit hit, chosenGroundCheckDistance,
        //    groundLayer,
        //    QueryTriggerInteraction.Ignore))
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 
            chosenGroundCheckDistance, groundLayer, QueryTriggerInteraction.Ignore))
        {
            if (Vector3.Dot(hit.normal, Vector3.up) > 0 &&
                IsNormalUnderSlope(hit.normal))
            {
                if (hit.distance > cc.skinWidth)
                {
                    cc.Move(Vector3.down * hit.distance);
                }
                GroundNormal = hit.normal;
                isGrounded = true;
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
        return angle <= cc.slopeLimit;
    }

    public void Jump(Vector3 jumpVelocity)
    {
        CharacterVelocity = jumpVelocity;
        lastTimeJump = Time.time;
    }
}
