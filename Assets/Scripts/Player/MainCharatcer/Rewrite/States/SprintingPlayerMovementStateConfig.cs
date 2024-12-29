using UnityEngine;
using UnityEngine.Windows;

public class SprintingPlayerMovementState : GroundedPlayerMovementState
{
    private float jumpHorizontalSpeed;
    private float toWalkSpeedThreshold;

    public SprintingPlayerMovementState(SprintingPlayerMovementStateConfig config)
        : base(config)
    {
        jumpHorizontalSpeed = config.JumpHorizontalSpeed;
        toWalkSpeedThreshold = config.ToWalkSpeedThreshold;
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
                velocity = velocity.normalized * jumpHorizontalSpeed;
                movementController.CharacterVelocity = velocity;

                movementController.Jump(ComputeJumpVelocity(deltaTime));
            }
            movementController.SetCurrentState(airStateHash);
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
            movementController.SetCurrentState(walkStateHash);
            movementController.GetCurrentState().UpdateMovementVelocity(deltaTime);
            return true;
        }

        if (inputController.crouchModifier)
        {
            movementController.SetCurrentState(crouchStateHash);
            movementController.GetCurrentState().UpdateMovementVelocity(deltaTime);
            return true;
        }

        return false;
    }

    public override void HandleObstacleAfterMovement(float deltaTime, in RaycastHit hit)
    {
        if (movementController.CharacterVelocity.magnitude < toWalkSpeedThreshold)
        {
            inputController.sprintModifier = false;
            movementController.SetCurrentState(walkStateHash);
        }
    }

    public override void OnStateDeactivated(IPlayerMovementState nextState)
    {
        base.OnStateDeactivated(nextState);
    }
}

[CreateAssetMenu(fileName = "SprintingPlayerMovementStateConfig", menuName = "Character/Movement/Sprinting Move State")]
public class SprintingPlayerMovementStateConfig : GroundedPlayerMovementStateConfig
{
    [SerializeField]
    private float jumpHorizontalSpeed;
    public float JumpHorizontalSpeed => jumpHorizontalSpeed;

    [SerializeField]
    [Tooltip("Скорость, порог итоговой скорости, меньше которой персонаж переходит на шаг(например, после того как уперся в стену)")]
    private float toWalkSpeedThreshold;

    public float ToWalkSpeedThreshold => toWalkSpeedThreshold;

    protected override IPlayerMovementState CreateMovementState()
    {
        return new SprintingPlayerMovementState(this);
    }
}
