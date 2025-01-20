using UnityEngine;
using UnityEngine.ProBuilder;

public class AirPlayerMovementState : PlayerMovementStateBase
{
    private float airDelimitSpeed;
    public AirPlayerMovementState(PlayerMovementStateConfig config) : base(config)
    {

    }

    #region [ Movement Cycle Methods ]
    // возвращает true при переходе в новое состояние
    public override bool MakeTransitions(float deltaTime)
    {
        bool isGrounded = movementController.GroundCheck();
        bool isJumping = inputController.jumpPressed;
        if (isGrounded && !isJumping)
        {
            movementController.SetCurrentState(PlayerMovementStateType.Walk);
            movementController.GetCurrentState().UpdateMovementVelocity(deltaTime);
            return true;
        }

        if (isJumping)
        {
            RaycastHit climbPointHit;
            if (movementController.TryGetClimbPoint(out climbPointHit))
            {
                movementController.SetCurrentState(PlayerMovementStateType.Climb);
                movementController.GetCurrentState().UpdateMovementVelocity(deltaTime);
                return true;
            }
        }

        return false;
    }
    protected override Vector3 ComputeVelocity(float deltaTime)
    {
        Vector3 WSmovementInput = inputController.GetMovementDirectionInTransformSpace(movementController.transform);

        Vector3 velocity = movementController.CharacterVelocity;

        velocity += WSmovementInput * config.Acceleration * deltaTime;
        float VertVelocity = velocity.y;
        Vector3 HorizontalVelocity = Vector3.ProjectOnPlane(velocity, Vector3.up);

        HorizontalVelocity = Vector3.ClampMagnitude(HorizontalVelocity, airDelimitSpeed);
        velocity = HorizontalVelocity + Vector3.up * VertVelocity;

        velocity += Vector3.down * config.GravityForce * deltaTime;

        return velocity;
    }
    public override void HandleObstacleAfterMovement(float deltaTime, in RaycastHit hit)
    {
        
    }
    #endregion

    public override void OnStateActivated(IPlayerMovementState prevState)
    {
        Vector3 velocity = movementController.CharacterVelocity;
        velocity.y = 0;
        airDelimitSpeed = velocity.magnitude > config.AirMaxSpeed ? velocity.magnitude : config.AirMaxSpeed;
    }
    public override void OnStateDeactivated(IPlayerMovementState nextState)
    {

    }
}