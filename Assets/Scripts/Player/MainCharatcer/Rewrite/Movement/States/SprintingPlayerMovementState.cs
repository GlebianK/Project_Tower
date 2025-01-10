using UnityEngine;
using UnityEngine.Windows;

public class SprintingPlayerMovementState : GroundedPlayerMovementState
{
    public SprintingPlayerMovementState(PlayerMovementStateConfig config)
        : base(config)
    {
        maxSpeed = config.SprintMaxSpeed;
    }

    public override bool MakeTransitions(float deltaTime)
    {
        bool isGrounded = movementController.GroundCheck();
        bool isJumping = inputController.jumpPressed;

        if (!isGrounded || isJumping)
        {
            if (isJumping)
            {
                Vector3 velocity = movementController.CharacterVelocity;
                velocity.y = 0;
                velocity = velocity.normalized * config.SprintJumpHorizontalSpeed;
                movementController.CharacterVelocity = velocity;

                movementController.Jump(ComputeJumpVelocity(config.SprintJumpVelocity, deltaTime));
            }
            movementController.SetCurrentState(PlayerMovementStateType.Air);
            movementController.GetCurrentState().UpdateMovementVelocity(deltaTime);
            return true;
        }

        Vector3 wsInput = inputController.GetMovementDirectionInTransformSpace(movementController.transform);
        bool isRunningForward = wsInput.magnitude > 0 &&
            Vector3.Dot(movementController.transform.forward, wsInput) > 0.70;

        if (!inputController.sprintModifier ||
            !isRunningForward) // проверка на то, что игрок все равно бежит вперед
        {
            if (!isRunningForward)
            {
                inputController.sprintModifier = false;
            }
            movementController.SetCurrentState(PlayerMovementStateType.Walk);
            movementController.GetCurrentState().UpdateMovementVelocity(deltaTime);
            return true;
        }

        if (inputController.crouchModifier)
        {
            PlayerMovementStateType newState =
                movementController.GetStateByType(PlayerMovementStateType.Slide) != null ?
                PlayerMovementStateType.Slide : PlayerMovementStateType.Crouch;

            movementController.SetCurrentState(newState);
            movementController.GetCurrentState().UpdateMovementVelocity(deltaTime);
            return true;
        }

        return false;
    }
    public override void HandleObstacleAfterMovement(float deltaTime, in RaycastHit hits)
    {

        if (movementController.CharacterVelocity.magnitude < config.SprintToWalkSpeedThreshold)
        {
            inputController.sprintModifier = false;
            movementController.SetCurrentState(PlayerMovementStateType.Walk);
        }
    }

    public override void OnStateDeactivated(IPlayerMovementState nextState)
    {
        base.OnStateDeactivated(nextState);
    }
}