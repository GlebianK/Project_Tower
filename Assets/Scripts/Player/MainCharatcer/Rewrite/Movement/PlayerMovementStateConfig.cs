using System.Collections.Generic;
using UnityEngine;

public enum PlayerMovementStateType
{
    Air,
    Walk,
    Crouch,
    Sprint,
    Slide,
    Climb
}

[CreateAssetMenu(fileName = "PlayerMovementConfig", menuName = "Player/Movement/MovementConfig")]
public class PlayerMovementStateConfig : ScriptableObject
{
    [Header("General")]
    [SerializeField] private float acceleration = 15;
    [SerializeField] private LayerMask groundLayer;
    [Header("Walk")]
    [SerializeField] private float walkMaxSpeed = 5;
    [SerializeField] private float jumpVelocity = 7;
    [Header("Air")]
    [SerializeField] private float gravityForce = 10;
    [SerializeField] private float airMaxSpeed = 5;
    [Header("Crouch")]
    [SerializeField] private float crouchMaxSpeed = 3;
    [SerializeField] private float crouchHeight = 1;
    [Header("Sprint")]
    [SerializeField] private bool enableSprint = true;
    [SerializeField] private float sprintMaxSpeed = 7;
    [SerializeField] private float sprintJumpVelocity = 5;
    [SerializeField] private float sprintJumpHorizontalSpeed = 9;
    [Tooltip("Скорость, порог итоговой скорости, меньше которой персонаж переходит на шаг(например, после того как уперся в стену)")]
    [SerializeField] private float sprintToWalkSpeedThreshold = 4;
    [Header("Slide")]
    [SerializeField] private bool enableSlide = true;
    [SerializeField] private float slideStartSpeed = 12;
    [SerializeField] private float slideDecceleration = 8;
    [Tooltip("Скорость, при достожении которой персонаж переходит в состояние присяда или бега")]
    [SerializeField] private float slideEndSpeedThreshold = 2;

    public float Acceleration => acceleration;
    public LayerMask GroundLayer => groundLayer;
    public float WalkMaxSpeed => walkMaxSpeed;
    public float JumpVelocity => jumpVelocity;
    public float GravityForce => gravityForce;
    public float AirMaxSpeed => airMaxSpeed;
    public float CrouchMaxSpeed => crouchMaxSpeed;
    public float CrouchHeight => crouchHeight;
    public float SprintMaxSpeed => sprintMaxSpeed;
    public float SprintJumpVelocity => sprintJumpVelocity;
    public float SprintJumpHorizontalSpeed => sprintJumpHorizontalSpeed;
    public float SprintToWalkSpeedThreshold => sprintToWalkSpeedThreshold;
    public float SlideStartSpeed => slideStartSpeed;
    public float SlideDecceleration => slideDecceleration;
    public float SlideEndSpeedThreshold => slideEndSpeedThreshold;

    public IEnumerable<KeyValuePair<PlayerMovementStateType, IPlayerMovementState>> CreateAllMovementStates(
        PlayerMovementStateMachine machine,
        MovementInputEventHandler inputHandler)
    {
        yield return CreateMovementStateInstance(PlayerMovementStateType.Air, machine, inputHandler);
        yield return CreateMovementStateInstance(PlayerMovementStateType.Walk, machine, inputHandler);
        yield return CreateMovementStateInstance(PlayerMovementStateType.Crouch, machine, inputHandler);
        if (enableSprint)
            yield return CreateMovementStateInstance(PlayerMovementStateType.Sprint, machine, inputHandler);
        if (enableSlide)
            yield return CreateMovementStateInstance(PlayerMovementStateType.Slide, machine, inputHandler);
    }

    private KeyValuePair<PlayerMovementStateType, IPlayerMovementState> CreateMovementStateInstance(
        PlayerMovementStateType type,
        PlayerMovementStateMachine machine, 
        MovementInputEventHandler inputHandler)
    {
        IPlayerMovementState newState = CreateMovementState(type);
        newState.InitializeContext(machine, inputHandler);
        return new KeyValuePair<PlayerMovementStateType, IPlayerMovementState>(type, newState);
    }
    private IPlayerMovementState CreateMovementState(PlayerMovementStateType type)
    {
        switch (type)
        {
            case PlayerMovementStateType.Air:
                return new AirPlayerMovementState(this);
            case PlayerMovementStateType.Walk:
                return new GroundedPlayerMovementState(this);
            case PlayerMovementStateType.Crouch:
                return new CrouchedPlayerMovementState(this);
            case PlayerMovementStateType.Sprint:
                return new SprintingPlayerMovementState(this);
            case PlayerMovementStateType.Slide:
                return new SlidePlayerMovementState(this);
            default:
                throw new System.ArgumentException();
                return null;
        }
    }
}
