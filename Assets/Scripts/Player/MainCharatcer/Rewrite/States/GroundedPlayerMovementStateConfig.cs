using NUnit.Framework.Interfaces;
using UnityEngine;

public class GroundedPlayerMovementState : PlayerMovementStateBase
{
    protected float acceleration;
    protected float maxSpeed;
    protected float jumpVelocity;

    protected int airStateHash;
    protected int crouchStateHash;
    protected int sprintStateHash;
    protected int walkStateHash;

    public GroundedPlayerMovementState(GroundedPlayerMovementStateConfig config)
    {
        acceleration = config.Acceleration;
        maxSpeed = config.MaxSpeed;
        jumpVelocity = config.JumpVelocity;

        airStateHash = config.AirStateHash;
        crouchStateHash = config.CrouchStateHash;
        sprintStateHash = config.SprintStateHash;
        walkStateHash = config.WalkStateHash;
    }

    #region [ Movement Cycle Methods ]
    // возвращает true, если был совершен переход в новое состояние
    public override bool MakeTransitions(float deltaTime)
    {
        bool isGrounded = movementController.GroundCheck();
        bool isJumping = inputController.jumpPressed;

        if (!isGrounded || isJumping)
        {
            if (isJumping)
            {
                movementController.Jump(ComputeJumpVelocity(deltaTime));
            }
            movementController.SetCurrentState(airStateHash);
            movementController.GetCurrentState().UpdateMovementVelocity(deltaTime);
            return true;
        }

        if (inputController.crouchModifier)
        {
            movementController.SetCurrentState(crouchStateHash);
            movementController.GetCurrentState().UpdateMovementVelocity(deltaTime);
            return true;
        }

        Vector3 wsInput = inputController.GetMovementDirectionInTransformSpace(movementController.transform);
        bool isRunningForward = wsInput.magnitude > 0 &&
            Vector3.Dot(movementController.transform.forward, wsInput) > 0.70;

        if (inputController.sprintModifier &&
            isRunningForward)
        {
            if (isRunningForward)
            {
                movementController.SetCurrentState(sprintStateHash);
                movementController.GetCurrentState().UpdateMovementVelocity(deltaTime);
                return true;
            }
            else
            {
                // TODO Dash transition
                inputController.sprintModifier = false;
            }
           
        }

        return false;
    }
    public virtual Vector3 ComputeJumpVelocity(float deltaTime)
    {
        Vector3 velocity = movementController.CharacterVelocity;
        velocity.y = jumpVelocity;
        return velocity;
    }
    protected override Vector3 ComputeVelocity(float deltaTime)
    {
        Vector3 WSinput = inputController.GetMovementDirectionInTransformSpace(movementController.transform);
        Vector3 targetVelocity = WSinput * maxSpeed;
        targetVelocity = movementController.GetDirectionOnSlope(targetVelocity.normalized, movementController.GroundNormal) * targetVelocity.magnitude;
        Vector3 resultVelocity = Vector3.Lerp(movementController.CharacterVelocity, targetVelocity, acceleration * deltaTime);
        return AvoidGroundJiggleForVelocity(resultVelocity, deltaTime);
    }
    protected Vector3 AvoidGroundJiggleForVelocity(Vector3 velocityToFix, float deltaTime)
    {
        return velocityToFix + movementController.GetDirectionOnSlope(Vector3.down, movementController.GroundNormal) * 0.05f / deltaTime;
    }
    public override void HandleObstacleAfterMovement(float deltaTime, in RaycastHit hit)
    {
        // ignore
    }
    #endregion
    public override void OnStateActivated(IPlayerMovementState prevState)
    {
        
    }
    public override void OnStateDeactivated(IPlayerMovementState nextState)
    {
        movementController.CharacterVelocity -= Vector3.down * 0.05f / Time.deltaTime;
    }
}

[CreateAssetMenu(fileName = "GroundPlayerMovementState", menuName = "Character/Movement/Ground Move State", order = 0)]
public class GroundedPlayerMovementStateConfig : PlayerMovementStateConfigBase
{
    [SerializeField] private float acceleration;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float jumpVelocity;

    [SerializeField] private string airStateName;
    [SerializeField] private string crouchStateName;
    [SerializeField] private string sprintStateName;
    [SerializeField] private string walkStateName;

    public float Acceleration => acceleration;
    public float MaxSpeed => maxSpeed;
    public float JumpVelocity => jumpVelocity;

    public string AirStateName => airStateName;
    public int AirStateHash => airStateName.GetHashCode();
    public string CrouchStateName => crouchStateName;
    public int CrouchStateHash => crouchStateName.GetHashCode();
    public string SprintStateName => sprintStateName;
    public int SprintStateHash => sprintStateName.GetHashCode();
    public string WalkStateName => walkStateName;
    public int WalkStateHash => walkStateName.GetHashCode();

    protected override IPlayerMovementState CreateMovementState()
    {
        return new GroundedPlayerMovementState(this);
    }
}
