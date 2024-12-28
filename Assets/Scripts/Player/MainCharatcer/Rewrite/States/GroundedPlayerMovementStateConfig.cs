using UnityEngine;

public class GroundedPlayerMovementState : PlayerMovementStateBase
{
    private float acceleration;
    private float maxSpeed;
    private float jumpVelocity;

    private int airStateHash;
    private int crouchStateHash;
    private int sprintStateHash;

    public GroundedPlayerMovementState(GroundedPlayerMovementStateConfig config)
    {
        acceleration = config.Acceleration;
        maxSpeed = config.MaxSpeed;
        jumpVelocity = config.JumpVelocity;

        airStateHash = config.AirStateHash;
        crouchStateHash = config.CrouchStateHash;
        sprintStateHash = config.SprintStateHash;
    }

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

        return false;
    }

    public virtual Vector3 ComputeJumpVelocity(float deltaTime)
    {
        Vector3 velocity = movementController.CharacterVelocity;
        velocity.y = jumpVelocity;
        return velocity;
    }

    protected Vector3 AvoidGroundJiggleForVelocity(Vector3 velocityToFix, float deltaTime)
    {
        return velocityToFix + movementController.GetDirectionOnSlope(Vector3.down, movementController.GroundNormal) * 0.05f / deltaTime;
    }

    protected override Vector3 ComputeVelocity(float deltaTime)
    {
        Vector3 WSinput = inputController.GetMovementDirectionInTransformSpace(movementController.transform);
        Vector3 targetVelocity = WSinput * maxSpeed;
        targetVelocity = movementController.GetDirectionOnSlope(targetVelocity.normalized, movementController.GroundNormal) * targetVelocity.magnitude;
        Vector3 resultVelocity = Vector3.Lerp(movementController.CharacterVelocity, targetVelocity, acceleration * deltaTime);
        return AvoidGroundJiggleForVelocity(resultVelocity, deltaTime);
    }

    public override void HandleObstacleAfterMovement(float deltaTime, in RaycastHit hit)
    {
        // ignore
    }

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
    [SerializeField]
    private float _acceleration;
    public float Acceleration => _acceleration;
    [SerializeField]
    private float _maxSpeed;
    public float MaxSpeed => _maxSpeed;
    [SerializeField]
    private float _jumpVelocity;
    public float JumpVelocity => _jumpVelocity;
    [SerializeField]
    private float _capsuleHeight;
    public float CapsuleHeight => _capsuleHeight;


    [SerializeField]
    private string _airStateName;
    public string AirStateName => _airStateName;
    public int AirStateHash => _airStateName.GetHashCode();
    [SerializeField]
    private string _crouchStateName;
    public string CrouchStateName => _crouchStateName;
    public int CrouchStateHash => _crouchStateName.GetHashCode();
    [SerializeField]
    private string _sprintStateName;
    public string SprintStateName => _sprintStateName;
    public int SprintStateHash => _sprintStateName.GetHashCode();

    protected override IPlayerMovementState CreateMovementState()
    {
        return new GroundedPlayerMovementState(this);
    }
}
