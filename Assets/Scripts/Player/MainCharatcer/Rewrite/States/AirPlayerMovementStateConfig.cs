using UnityEngine;

public class AirPlayerMovementState : PlayerMovementStateBase
{
    private float _acceleration;
    private float _airMaxSpeed;
    private float _gravityForce;

    private int _groundStateHash;

    private float _airDelimitSpeed;

    public AirPlayerMovementState(AirPlayerMovementStateConfig config)
    {
        _acceleration = config.Acceleration;
        _airMaxSpeed = config.AirMaxSpeed;
        _gravityForce = config.GravityForce;
        _groundStateHash = config.GroundStateHash;
    }

    public override void HandleObstacleAfterMovement(float deltaTime, in RaycastHit hit)
    {
        
    }

    public override void OnStateActivated(IPlayerMovementState prevState)
    {
        Vector3 velocity = movementController.CharacterVelocity;
        velocity.y = 0;
        _airDelimitSpeed = velocity.magnitude > _airMaxSpeed ? velocity.magnitude : _airMaxSpeed;
    }

    public override void OnStateDeactivated(IPlayerMovementState nextState)
    {
        
    }

    // возвращает true при переходе в новое состояние
    public override bool MakeTransitions(float deltaTime)
    {
        bool isGrounded = movementController.GroundCheck();
        bool isJumping = inputController.jumpPressed;
        if (isGrounded && !isJumping)
        {
            movementController.SetCurrentState(_groundStateHash);
            movementController.GetCurrentState().UpdateMovementVelocity(deltaTime);
            return true;
        }

        return false;
    }

    protected override Vector3 ComputeVelocity(float deltaTime)
    {
        Vector3 WSmovementInput = inputController.GetMovementDirectionInTransformSpace(movementController.transform);

        Vector3 velocity = movementController.CharacterVelocity;

        velocity += WSmovementInput * _acceleration * deltaTime;
        float VertVelocity = velocity.y;
        Vector3 HorizontalVelocity = Vector3.ProjectOnPlane(velocity, Vector3.up);

        HorizontalVelocity = Vector3.ClampMagnitude(HorizontalVelocity, _airDelimitSpeed);
        velocity = HorizontalVelocity + Vector3.up * VertVelocity;

        velocity += Vector3.down * _gravityForce * deltaTime;

        return velocity;
    }
}

[CreateAssetMenu(fileName = "AirPlayerMovementState", menuName = "Character/Movement/Air Move State")]
public class AirPlayerMovementStateConfig : PlayerMovementStateConfigBase
{
    [SerializeField]
    private float _acceleration;
    public float Acceleration => _acceleration;
    [SerializeField]
    private float _gravityForce;
    public float GravityForce => _gravityForce;
    [SerializeField]
    private float _airMaxSpeed;
    public float AirMaxSpeed => _airMaxSpeed;
    [SerializeField]
    private string _groundStateName;
    public string GroundStateName => _groundStateName;
    public int GroundStateHash => _groundStateName.GetHashCode();
    protected override IPlayerMovementState CreateMovementState()
    {
        return new AirPlayerMovementState(this);
    }
}
