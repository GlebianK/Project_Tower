using NUnit.Framework.Interfaces;
using UnityEngine;
using UnityEngine.ProBuilder;

public class GroundedPlayerMovementState : PlayerMovementStateBase
{
    protected float maxSpeed;
    public GroundedPlayerMovementState(PlayerMovementStateConfig config) : base(config)
    {
        maxSpeed = config.WalkMaxSpeed;
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
                movementController.Jump(ComputeJumpVelocity(config.JumpVelocity, deltaTime));
            }
            movementController.SetCurrentState(PlayerMovementStateType.Air);
            movementController.GetCurrentState().UpdateMovementVelocity(deltaTime);
            return true;
        }

        if (inputController.crouchModifier)
        {
            movementController.SetCurrentState(PlayerMovementStateType.Crouch);
            movementController.GetCurrentState().UpdateMovementVelocity(deltaTime);
            return true;
        }

        Vector3 wsInput = inputController.GetMovementDirectionInTransformSpace(movementController.transform);
        bool isRunningForward = wsInput.magnitude > 0 &&
            Vector3.Dot(movementController.transform.forward, wsInput) > 0.70;

        if (inputController.sprintModifier)
        {
            if (isRunningForward)
            {
                movementController.SetCurrentState(PlayerMovementStateType.Sprint);
                movementController.GetCurrentState().UpdateMovementVelocity(deltaTime);
                return true;
            }
            else
            {
                // TODO Dash transition
                inputController.sprintModifier = false;
                movementController.SetCurrentState(PlayerMovementStateType.Dash);
                movementController.GetCurrentState().UpdateMovementVelocity(deltaTime);
                return true;
            }
           
        }

        return false;
    }
    public virtual Vector3 ComputeJumpVelocity(float upVelocity, float deltaTime)
    {
        Vector3 velocity = movementController.CharacterVelocity;
        velocity.y = upVelocity;
        return velocity;
    }
    protected override Vector3 ComputeVelocity(float deltaTime)
    {
        Vector3 WSinput = inputController.GetMovementDirectionInTransformSpace(movementController.transform);
        Vector3 targetVelocity = WSinput * maxSpeed;
        targetVelocity = movementController.GetDirectionOnSlope(targetVelocity.normalized, movementController.GroundNormal) * targetVelocity.magnitude;
        Vector3 resultVelocity = Vector3.Lerp(movementController.CharacterVelocity, targetVelocity, config.Acceleration * deltaTime);
        return resultVelocity;
    }

    public override void HandleObstacleAfterMovement(float deltaTime, in RaycastHit hit)
    {

    }
    #endregion
    public override void OnStateActivated(IPlayerMovementState prevState)
    {
        if (inputController.GetMovementDirectionRaw().y < 1)
        {
            inputController.sprintModifier = false;
        }
    }
    public override void OnStateDeactivated(IPlayerMovementState nextState)
    {
        
    }
}