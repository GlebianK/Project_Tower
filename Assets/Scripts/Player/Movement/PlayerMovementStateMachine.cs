using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovementStateMachine : MonoBehaviour
{
    [SerializeField] private PlayerMovementStateConfig configuration;
    public Vector3 CharacterVelocity { get; set; }

    [SerializeField] private MovementInputEventHandler movementInputEventHandler;
    [SerializeField] private PlayerMovementStateType startState;

    private Dictionary<PlayerMovementStateType, IPlayerMovementState> movementStates;
    private PlayerMovementStateType currentMovementState;

    public UnityEvent<PlayerMovementStateMachine, PlayerMovementStateType> StateChanged;

    // нужно для предотвращения "приклеивания" игрока к полу сразу после прыжка
    private float lastTimeJump;
    // если есть, пытается восстановить высоту персонажа
    private Coroutine uncrouchCoroutine = null;
    private bool lastGroundedCheck = false;
    public CharacterController cc { get; private set; }
    public Vector3 GroundNormal { get; private set; }
    public float PlayerDefaultHeight { get; private set; }

    public PlayerMovementStateConfig Properties => configuration;

    private void Awake()
    {
        cc = GetComponent<CharacterController>();
        lastTimeJump = -1;
    }
    private void Start()
    {
        movementStates = configuration.CreateAllMovementStates(this, movementInputEventHandler).ToDictionary(
            elem => elem.Key, elem => elem.Value);

        SetCurrentState(startState);
        CharacterVelocity = Vector3.zero;
        GroundNormal = Vector3.up;
        PlayerDefaultHeight = cc.height;
    }
    private void Update()
    {
        PlayerMovementStateType oldState = GetCurrentStateType();

        GetCurrentState().UpdateMovementVelocity(Time.deltaTime);

        Vector3 startHandlePosition = transform.position;
        Vector3 moveVec = CharacterVelocity * Time.deltaTime;
        if (cc.enabled)
        {
            cc.Move(moveVec);
        }
        else
        {
            transform.position += moveVec;
        }
        RaycastHit hitResult;

        if (TryHandleObstacle(startHandlePosition, moveVec, out hitResult))
        {
            CharacterVelocity = Vector3.ProjectOnPlane(CharacterVelocity, hitResult.normal);
        }

        GetCurrentState().HandleObstacleAfterMovement(Time.deltaTime, hitResult);

        if (oldState != GetCurrentStateType())
        {
            StateChanged.Invoke(this, GetCurrentStateType());
        }
        movementInputEventHandler.jumpPressed = false;
    }

    #region [ States Management Methods ]
    public IPlayerMovementState GetCurrentState()
    {
        return movementStates[currentMovementState];
    }
    public PlayerMovementStateType GetCurrentStateType() => currentMovementState;
    public IPlayerMovementState GetStateByType(PlayerMovementStateType type)
    {
        return movementStates[type];
    }
    public void BlockMovementState(PlayerMovementStateType type)
    {
        GetStateByType(type).Block();
    }
    public void UnblockMovementState(PlayerMovementStateType type)
    {
        GetStateByType(type).Unblock();
    }
    public void SetCurrentState(PlayerMovementStateType state)
    {
        IPlayerMovementState newState = movementStates[state];
        if (newState == GetCurrentState())
            return;

        IPlayerMovementState oldState = movementStates[currentMovementState];
        currentMovementState = state;
        oldState?.OnStateDeactivated(newState);
        newState?.OnStateActivated(oldState);
    }
    #endregion

    #region [ Height Control Methods ]
    private void UpdateHeight(float height)
    {
        cc.height = height;
        cc.center = Vector3.up * cc.height * 0.5f;
    }
    public bool CanSetHeight(float height)
    {
        /*Collider[] standingOverlaps = */
        return !Physics.CheckCapsule(
                    transform.position + Vector3.up * cc.radius,
                    transform.position + Vector3.up * (height - cc.radius),
                    cc.radius,
                    Properties.GroundLayer,
                    QueryTriggerInteraction.Ignore);

        //foreach (Collider c in standingOverlaps)
        //{
        //    if (c != this.cc) return false;
        //}
        //return true;
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
    private IEnumerator TrySetHeightForWhileCoroutine(float height)
    {
        while (!CanSetHeight(height))
        {
            yield return null;
        }

        UpdateHeight(height);
    }
    #endregion

    #region [ Obstacle resolve ]
    private bool TryHandleObstacle(Vector3 startPoint, Vector3 moveVector, out RaycastHit hit)
    {
        Vector3 capsuleBottomBeforeMove = startPoint + transform.up * cc.radius;
        Vector3 capsuleTopBeforeMove = startPoint + transform.up * (cc.height - cc.radius);

        return Physics.CapsuleCast(
                capsuleBottomBeforeMove,
                capsuleTopBeforeMove,
                cc.radius,
                moveVector.normalized,
                out hit,
                moveVector.magnitude,
                configuration.GroundLayer, QueryTriggerInteraction.Ignore); 
    }
    #endregion

    #region [ Ground, Jump and Climb ]
    public bool GroundCheck()
    {
        float chosenGroundCheckDistance = cc.skinWidth + (lastGroundedCheck ? cc.stepOffset : 0.01f);
        GroundNormal = Vector3.up;

        bool isGrounded = false;
        float afterJumpTime = Time.time - lastTimeJump;
        if (afterJumpTime < 0.12f)
        {
            return false;
        }

        Vector3 capsuleBottom = transform.position + Vector3.up * cc.radius;
        Vector3 capsuleTop = transform.position + Vector3.up * (cc.height - cc.radius);

        if (Physics.CapsuleCast(capsuleBottom, capsuleTop, cc.radius, Vector3.down, out RaycastHit hit,
            chosenGroundCheckDistance, configuration.GroundLayer, QueryTriggerInteraction.Ignore))
        {
            if (Vector3.Dot(hit.normal, Vector3.up) > 0 &&
                IsNormalUnderSlope(hit.normal))
            {
                GroundNormal = hit.normal;
                isGrounded = true;
                if (hit.distance > cc.skinWidth)
                {
                    cc.Move(Vector3.down * (hit.distance - cc.skinWidth));
                }
            }
        }
        lastGroundedCheck = isGrounded;
        return isGrounded;

    }
    public void Jump(Vector3 jumpVelocity)
    {
        CharacterVelocity = jumpVelocity;
        lastTimeJump = Time.time;
        movementInputEventHandler.jumpPressed = false;
        lastGroundedCheck = false;
    }

    private bool IsClimbDestinationCorrect(in RaycastHit hit)
    {
        Transform playerTransform = transform;
        Vector3 verticalMoveVector = Vector3.up * (hit.point.y - playerTransform.position.y);
        //Проверяем, можно ли подняться наверх
        if (Physics.CapsuleCast(
            playerTransform.position + Vector3.up * cc.radius,
            playerTransform.position + Vector3.up * (Properties.CrouchHeight - cc.radius),
            cc.radius - 0.05f,
            verticalMoveVector.normalized,
            verticalMoveVector.magnitude,
            Properties.GroundLayer,
            QueryTriggerInteraction.Ignore
            ))
        {
            return false;
        }

        Vector3 horizontalMoveVector = hit.point - playerTransform.position;
        horizontalMoveVector.y = 0;
        //проверяем, можно ли переместиться по горизонтали
        if (Physics.CapsuleCast(

            playerTransform.position + verticalMoveVector + Vector3.up * (cc.radius + cc.skinWidth),
            playerTransform.position + verticalMoveVector + Vector3.up * (Properties.CrouchHeight - cc.radius),
            cc.radius - 0.05f,
            horizontalMoveVector.normalized,
            horizontalMoveVector.magnitude,
            Properties.GroundLayer,
            QueryTriggerInteraction.Ignore
            ))
        {
            return false;
        }

        // проверяем, может ли капсула встать в положенное место
        if (!Physics.CheckCapsule(
            hit.point + Vector3.up * (cc.radius + cc.skinWidth + cc.stepOffset),
            hit.point + Vector3.up * (Properties.CrouchHeight - cc.radius + cc.skinWidth),
            cc.radius, Properties.GroundLayer,
            QueryTriggerInteraction.Ignore))
        {
            return true;
        }

        return false;
    }

    public bool TryGetClimbPoint(out RaycastHit climbHit)
    {
        climbHit = new RaycastHit();

        Transform playerTransform = transform;

        float climbDownRaycastForwardOffset = cc.radius;

        RaycastHit climbDestinationHit = new RaycastHit();

        Vector3 climbDownRaycastStart = playerTransform.position
            + Vector3.up * Properties.ClimbCheckpointHeight;
        
        while (climbDownRaycastForwardOffset < Properties.ClimbCheckForwardRange + cc.radius)
        {
            if (Physics.Raycast(
                climbDownRaycastStart + playerTransform.forward * climbDownRaycastForwardOffset,
                Vector3.down,
                out climbDestinationHit,
                Properties.ClimbDownCheckRange,
                Properties.ClimbObstacleLayer,
                QueryTriggerInteraction.Collide
                ))
            {
                if (IsClimbDestinationCorrect(in climbDestinationHit))
                {
                    climbHit = climbDestinationHit;
                    return true;
                }
            }
            climbDownRaycastForwardOffset += 0.1f;
        }

        if (climbDestinationHit.collider == null)
        {
            climbDownRaycastForwardOffset = Properties.ClimbCheckForwardRange + cc.radius;
            if (!Physics.Raycast(
                climbDownRaycastStart + playerTransform.forward * climbDownRaycastForwardOffset,
                Vector3.down,
                out climbDestinationHit,
                Properties.ClimbDownCheckRange,
                Properties.ClimbObstacleLayer,
                QueryTriggerInteraction.Collide
                ))
            {
                return false;
            }
            
            if (IsClimbDestinationCorrect(in climbDestinationHit))
            {
                climbHit = climbDestinationHit;
                return true;
            }
        }

        return false;

    }
    #endregion

    #region [ Direction Correction Methods ]

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

    
    #endregion
}
