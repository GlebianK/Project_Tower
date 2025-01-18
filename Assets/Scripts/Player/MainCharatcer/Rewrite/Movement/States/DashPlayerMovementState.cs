using UnityEngine;

public class DashPlayerMovementState : PlayerMovementStateBase
{
    private float time = 0;
    private Vector3 dashVector = Vector3.zero;
    public DashPlayerMovementState(PlayerMovementStateConfig config) : base(config)
    {
    }

    public override void HandleObstacleAfterMovement(float deltaTime, in RaycastHit hit)
    {
        if (time > config.DashTime)
        {
            movementController.CharacterVelocity = movementController.CharacterVelocity.normalized * config.AirMaxSpeed;
            movementController.SetCurrentState(PlayerMovementStateType.Air);
        }
    }

    public override bool MakeTransitions(float deltaTime)
    {
        return false;
    }

    public override void OnStateActivated(IPlayerMovementState prevState)
    {
        time = 0;
        dashVector = inputController.GetMovementDirectionInTransformSpace(movementController.transform);
    }

    public override void OnStateDeactivated(IPlayerMovementState nextState)
    {
        
    }

    protected override Vector3 ComputeVelocity(float deltaTime)
    {
        movementController.GroundCheck();
        Vector3 resultDirection = movementController.GetDirectionOnSlope(dashVector.normalized, movementController.GroundNormal.normalized).normalized;

        time += deltaTime;
        return resultDirection * config.DashRange / config.DashTime;
    }
}
