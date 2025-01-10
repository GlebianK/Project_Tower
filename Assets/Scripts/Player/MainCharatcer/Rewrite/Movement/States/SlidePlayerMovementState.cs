using UnityEngine;

public class SlidePlayerMovementState : PlayerMovementStateBase
{
    private Vector3 slideDirection;
    private float slideCurrentSpeed;

    public SlidePlayerMovementState(PlayerMovementStateConfig config) : base(config)
    {
        
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

        if (slideCurrentSpeed < config.SlideEndSpeedThreshold)
        {
            if (inputController.crouchModifier)
            {
                inputController.sprintModifier = false;
                
            }
            movementController.SetCurrentState(PlayerMovementStateType.Crouch);
            movementController.GetCurrentState().UpdateMovementVelocity(deltaTime);
            return true;
        }

        return false;
    }
    protected override Vector3 ComputeVelocity(float deltaTime)
    {
        Vector3 velocityDirection = movementController.GetDirectionOnSlope(slideDirection, movementController.GroundNormal).normalized;
        Vector3 resultVelocity = velocityDirection * slideCurrentSpeed;
        Debug.Log($"Result velocity: {resultVelocity}");
        slideCurrentSpeed -= config.SlideDecceleration * deltaTime;
        return resultVelocity;
    }
    public override void HandleObstacleAfterMovement(float deltaTime, in RaycastHit hit)
    {
        movementController.CharacterVelocity = Vector3.ProjectOnPlane(
            movementController.CharacterVelocity, hit.normal);
        if (hit.transform != null)
        {
            slideCurrentSpeed = movementController.CharacterVelocity.magnitude;
        }
    }

    public override void OnStateActivated(IPlayerMovementState prevState)
    {
        slideDirection = inputController.GetMovementDirectionInTransformSpace(movementController.transform);
        slideCurrentSpeed = config.SlideStartSpeed;
        movementController.SetHeight(config.CrouchHeight, false);
    }
    public override void OnStateDeactivated(IPlayerMovementState nextState)
    {
        movementController.ResetHeight();
    }
}