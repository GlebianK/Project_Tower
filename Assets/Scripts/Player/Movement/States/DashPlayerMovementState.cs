using UnityEngine;

public class DashPlayerMovementState : PlayerMovementStateBase
{
    private float time = 0;
    private Vector3 dashVector = Vector3.zero;

    private Health health;

    public DashPlayerMovementState(PlayerMovementStateConfig config) : base(config)
    {
        
    }

    public override void HandleObstacleAfterMovement(float deltaTime, in RaycastHit hit)
    {
       
    }

    public override bool MakeTransitions(float deltaTime)
    {
        if (time > config.DashTime)
        {
            movementController.CharacterVelocity = movementController.CharacterVelocity.normalized * config.AirMaxSpeed;
            movementController.SetCurrentState(PlayerMovementStateType.Air);
            movementController.GetCurrentState().UpdateMovementVelocity(deltaTime);
            return true;
        }
        return false;
    }

    public override void OnStateActivated(IPlayerMovementState prevState)
    {
        time = 0;
        dashVector = inputController.GetMovementDirectionInTransformSpace(movementController.transform);
        if (health == null)
        {
            health = movementController.GetComponent<Health>();
        }
        health.IsInvulnerable = true;    
    }

    public override void OnStateDeactivated(IPlayerMovementState nextState)
    {

        health.IsInvulnerable = false;
    }

    protected override Vector3 ComputeVelocity(float deltaTime)
    {
        movementController.GroundCheck();
        Vector3 resultDirection = movementController.GetDirectionOnSlope(dashVector.normalized, movementController.GroundNormal.normalized).normalized;

        time += deltaTime;
        return resultDirection * config.DashRange / config.DashTime;
    }
}
