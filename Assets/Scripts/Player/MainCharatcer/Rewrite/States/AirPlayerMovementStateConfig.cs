using UnityEngine;

public class AirPlayerMovementState : PlayerMovementStateBase
{
    private float acceleration;
    private float airMaxSpeed;
    private float gravityForce;
    private float airDelimitSpeed;

    private int groundStateHash;

    public AirPlayerMovementState(AirPlayerMovementStateConfig config)
    {
        acceleration = config.Acceleration;
        airMaxSpeed = config.AirMaxSpeed;
        gravityForce = config.GravityForce;
        groundStateHash = config.GroundStateHash;
    }

    #region [ Movement Cycle Methods ]
    // возвращает true при переходе в новое состояние
    public override bool MakeTransitions(float deltaTime)
    {
        bool isGrounded = movementController.GroundCheck();
        bool isJumping = inputController.jumpPressed;
        if (isGrounded && !isJumping)
        {
            movementController.SetCurrentState(groundStateHash);
            movementController.GetCurrentState().UpdateMovementVelocity(deltaTime);
            return true;
        }

        return false;
    }
    protected override Vector3 ComputeVelocity(float deltaTime)
    {
        Vector3 WSmovementInput = inputController.GetMovementDirectionInTransformSpace(movementController.transform);

        Vector3 velocity = movementController.CharacterVelocity;

        velocity += WSmovementInput * acceleration * deltaTime;
        float VertVelocity = velocity.y;
        Vector3 HorizontalVelocity = Vector3.ProjectOnPlane(velocity, Vector3.up);

        HorizontalVelocity = Vector3.ClampMagnitude(HorizontalVelocity, airDelimitSpeed);
        velocity = HorizontalVelocity + Vector3.up * VertVelocity;

        velocity += Vector3.down * gravityForce * deltaTime;

        return velocity;
    }
    public override void HandleObstacleAfterMovement(float deltaTime, in RaycastHit hit)
    {
        
    }
    #endregion

    public override void OnStateActivated(IPlayerMovementState prevState)
    {
        Vector3 velocity = movementController.CharacterVelocity;
        if (inputController.jumpPressed)
            velocity.y = 0;
        airDelimitSpeed = velocity.magnitude > airMaxSpeed ? velocity.magnitude : airMaxSpeed;
    }
    public override void OnStateDeactivated(IPlayerMovementState nextState)
    {

    }
}

[CreateAssetMenu(fileName = "AirPlayerMovementState", menuName = "Character/Movement/Air Move State")]
public class AirPlayerMovementStateConfig : PlayerMovementStateConfigBase
{
    [SerializeField] private float acceleration;
    [SerializeField] private float gravityForce;
    [SerializeField] private float airMaxSpeed;
    [SerializeField] private string groundStateName;

    public float Acceleration => acceleration;
    public float GravityForce => gravityForce;
    public float AirMaxSpeed => airMaxSpeed;

    public string GroundStateName => groundStateName;
    public int GroundStateHash => groundStateName.GetHashCode();

    protected override IPlayerMovementState CreateMovementState()
    {
        return new AirPlayerMovementState(this);
    }
}
