using UnityEngine;

public class SlidePlayerMovementState : PlayerMovementStateBase
{
    private float startSpeed;
    private float decceleration;
    private float endSpeedThreshold;
    private float playerHeight;

    private int airStateHash;
    private int crouchStateHash;
    private int sprintStateHash;

    private Vector3 slideDirection;
    private float slideCurrentSpeed;

    public SlidePlayerMovementState(SlidePlayerMovementStateConfig config)
    {
        startSpeed = config.StartSpeed;
        decceleration = config.Decceleration;
        endSpeedThreshold = config.EndSpeedThreshold;
        playerHeight = config.PlayerHeight;

        airStateHash = config.AirStateHash;
        crouchStateHash = config.CrouchStateHash;
        sprintStateHash = config.SprintStateHash;
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

        if (slideCurrentSpeed < endSpeedThreshold)
        {
            if (inputController.crouchModifier)
            {
                inputController.sprintModifier = false;
                movementController.SetCurrentState(crouchStateHash);
                movementController.GetCurrentState().UpdateMovementVelocity(deltaTime);
                return true;
            }
            else
            {
                movementController.SetCurrentState(sprintStateHash);
                movementController.GetCurrentState().UpdateMovementVelocity(deltaTime);
                return true;
            }
        }

        return false;
    }
    protected override Vector3 ComputeVelocity(float deltaTime)
    {
        Vector3 velocityDirection = movementController.GetDirectionOnSlope(slideDirection, movementController.GroundNormal).normalized;
        Vector3 resultVelocity = velocityDirection * slideCurrentSpeed;
        Debug.Log($"Result velocity: {resultVelocity}");
        slideCurrentSpeed -= decceleration * deltaTime;
        return resultVelocity;
    }
    public override void HandleObstacleAfterMovement(float deltaTime, in RaycastHit hit)
    {
        if (hit.transform != null)
        {
            slideCurrentSpeed = movementController.CharacterVelocity.magnitude;
        }
    }

    public override void OnStateActivated(IPlayerMovementState prevState)
    {
        slideDirection = inputController.GetMovementDirectionInTransformSpace(movementController.transform);
        slideCurrentSpeed = startSpeed;
        movementController.SetHeight(playerHeight, false);
    }
    public override void OnStateDeactivated(IPlayerMovementState nextState)
    {
        movementController.ResetHeight();
        movementController.CharacterVelocity -= Vector3.down * 0.05f / Time.deltaTime;
    }
}

[CreateAssetMenu(fileName = "SlidePlayerMovementStateConfig", menuName = "Character/Movement/Slide Movement State")]
public class SlidePlayerMovementStateConfig : PlayerMovementStateConfigBase
{
    [SerializeField] private float startSpeed;
    [SerializeField] private float decceleration;
    [Tooltip("Скорость, при достожении которой персонаж переходит в состояние присяда или бега")]
    [SerializeField] private float endSpeedThreshold;

    [SerializeField] private float playerHeight;

    [SerializeField] private string airState;
    [SerializeField] private string sprintState;
    [SerializeField] private string crouchState;

    public float StartSpeed => startSpeed;
    public float Decceleration => decceleration;
    public float EndSpeedThreshold => endSpeedThreshold;

    public float PlayerHeight => playerHeight;

    public string AirState => airState;
    public int AirStateHash => airState.GetHashCode();
    public string SprintState => sprintState;
    public int SprintStateHash => sprintState.GetHashCode();
    public string CrouchState => crouchState;
    public int CrouchStateHash => crouchState.GetHashCode();

    protected override IPlayerMovementState CreateMovementState()
    {
        return new SlidePlayerMovementState(this);
    }
}
