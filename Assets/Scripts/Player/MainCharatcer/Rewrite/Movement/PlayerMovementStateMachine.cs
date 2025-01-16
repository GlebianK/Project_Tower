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

    // нужно дл€ предотвращени€ "приклеивани€" игрока к полу сразу после прыжка
    private float lastTimeJump;
    // если есть, пытаетс€ восстановить высоту персонажа
    private Coroutine uncrouchCoroutine = null;

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

    private void OnDrawGizmos()
    {
        
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
        float chosenGroundCheckDistance = cc.skinWidth + cc.stepOffset;
        GroundNormal = Vector3.up;
        bool isGrounded = false;
        float afterJumpTime = Time.time - lastTimeJump;
        if (afterJumpTime < 0.12f)
        {
            return false;
        }

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 
            chosenGroundCheckDistance, configuration.GroundLayer, QueryTriggerInteraction.Ignore))
        {
            if (Vector3.Dot(hit.normal, Vector3.up) > 0 &&
                IsNormalUnderSlope(hit.normal))
            {
                GroundNormal = hit.normal;
                isGrounded = true;
            }
        }
        return isGrounded;

    }
    public void Jump(Vector3 jumpVelocity)
    {
        CharacterVelocity = jumpVelocity;
        lastTimeJump = Time.time;
    }

    public bool TryGetClimbPoint(out RaycastHit climbHit)
    {
        climbHit = new RaycastHit();

        Transform playerTransform = transform;

        Vector3 forwardCheckVector = playerTransform.forward * Properties.ClimbCheckForwardRange;

        RaycastHit[] hits = Physics.CapsuleCastAll(
            playerTransform.position + Vector3.up * (cc.radius + cc.skinWidth),
            playerTransform.position + Vector3.up * (cc.height - cc.radius),
            cc.radius,
            forwardCheckVector.normalized,
            forwardCheckVector.magnitude,
            Properties.ClimbObstacleLayer,
            QueryTriggerInteraction.Ignore);
        // провер€ем, есть ли впереди игрока какие то преп€тстви€ дл€ карабкань€
        foreach (RaycastHit hit in hits)
        {
            Vector3 climbDownRaycastStart = playerTransform.position
                + playerTransform.forward * (hit.distance + cc.radius)
                + Vector3.up * Properties.ClimbCheckpointHeight + transform.forward * cc.radius;

            RaycastHit climbDownRaycast;

            if (Physics.Raycast(
                climbDownRaycastStart,
                Vector3.down,
                out climbDownRaycast,
                Properties.ClimbDownCheckRange,
                Properties.ClimbObstacleLayer,
                QueryTriggerInteraction.Ignore
                ))
            {
                
                if (!Physics.CheckCapsule(
                    climbDownRaycast.point + Vector3.up * (cc.radius + cc.skinWidth + cc.stepOffset),
                    climbDownRaycast.point + Vector3.up * (Properties.CrouchHeight - cc.radius + cc.skinWidth),
                    cc.radius, Properties.GroundLayer,
                    QueryTriggerInteraction.Ignore))
                {
                    climbHit = climbDownRaycast;
                    return true;
                }
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
