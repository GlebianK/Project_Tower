using UnityEngine;

public class CrouchedPlayerMovementState : GroundedPlayerMovementState
{
    private float capsuleHeight;

    public CrouchedPlayerMovementState(CrouchedPlayerMovementStateConfig config)
        : base(config)
    {
        capsuleHeight = config.PlayerHeight;
    }

    public override bool MakeTransitions(float deltaTime)
    {
        bool isGrounded = movementController.GroundCheck();
        if (!isGrounded)
        {
            movementController.SetCurrentState(airStateHash);
            movementController.GetCurrentState().UpdateMovementVelocity(deltaTime);
            return true;
        }
        if (!inputController.crouchModifier &&
            movementController.CanSetHeight(movementController.PlayerDefaultHeight))
        {
            movementController.SetCurrentState(walkStateHash);
            movementController.GetCurrentState().UpdateMovementVelocity(deltaTime);
            return true;
        }

        return false;
    }

    public override void OnStateActivated(IPlayerMovementState prevState)
    {
        movementController.SetHeight(capsuleHeight, false);
        base.OnStateActivated(prevState);
    }
    public override void OnStateDeactivated(IPlayerMovementState nextState)
    {
        base.OnStateDeactivated(nextState);
        movementController.ResetHeight();
    }
}

[CreateAssetMenu(fileName = "CrouchedPlayerMovementStateConfig", menuName = "Character/Movement/Crouched Movement State")]
public class CrouchedPlayerMovementStateConfig : GroundedPlayerMovementStateConfig
{
    [SerializeField] private float playerHeight;

    public float PlayerHeight => playerHeight;

    protected override IPlayerMovementState CreateMovementState()
    {
        return new CrouchedPlayerMovementState(this);
    }
}
