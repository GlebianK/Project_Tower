using UnityEngine;

public class CrouchedPlayerMovementState : GroundedPlayerMovementState
{

    public CrouchedPlayerMovementState(PlayerMovementStateConfig config)
        : base(config)
    {
        maxSpeed = config.CrouchMaxSpeed;
    }

    public override bool MakeTransitions(float deltaTime)
    {
        bool isGrounded = movementController.GroundCheck();
        if (!isGrounded)
        {
            movementController.SetCurrentState(PlayerMovementStateType.Air);
            movementController.GetCurrentState().UpdateMovementVelocity(deltaTime);
            return true;
        }
        if (!inputController.crouchModifier &&
            movementController.CanSetHeight(movementController.PlayerDefaultHeight))
        {
            movementController.SetCurrentState(PlayerMovementStateType.Walk);
            movementController.GetCurrentState().UpdateMovementVelocity(deltaTime);
            return true;
        }

        return false;
    }

    public override void HandleObstacleAfterMovement(float deltaTime, in RaycastHit hit)
    {
        
    }

    public override void OnStateActivated(IPlayerMovementState prevState)
    {
        movementController.SetHeight(config.CrouchHeight, false);
        base.OnStateActivated(prevState);
    }
    public override void OnStateDeactivated(IPlayerMovementState nextState)
    {
        base.OnStateDeactivated(nextState);
        movementController.ResetHeight();
    }
}
