using UnityEngine;


public interface IPlayerMovementState
{
    void InitializeContext(
        PlayerMovementStateMachine machine,
        MovementInputEventHandler inputHandler);

    void UpdateMovementVelocity(float deltaTime);
    void HandleObstacleAfterMovement(float deltaTime, in RaycastHit hit);
    void OnStateActivated(IPlayerMovementState prevState);
    void OnStateDeactivated(IPlayerMovementState nextState);
}

public abstract class PlayerMovementStateBase : IPlayerMovementState
{
    private PlayerMovementStateMachine _machine;
    private MovementInputEventHandler _inputHandler;

    protected PlayerMovementStateMachine movementController => _machine;
    protected MovementInputEventHandler inputController => _inputHandler;

    public void InitializeContext(PlayerMovementStateMachine machine, MovementInputEventHandler inputHandler)
    {
        _machine = machine;
        _inputHandler = inputHandler;
    }
    public abstract void OnStateActivated(IPlayerMovementState prevState);
    public abstract void OnStateDeactivated(IPlayerMovementState nextState);

    // возвращает true, если необходимо перейти в другое состояние
    public abstract bool MakeTransitions(float deltaTime);

    public void UpdateMovementVelocity(float deltaTime)
    {
        if (MakeTransitions(deltaTime))
            return;

        _machine.CharacterVelocity = ComputeVelocity(deltaTime);
    }
    protected abstract Vector3 ComputeVelocity(float deltaTime);

    public abstract void HandleObstacleAfterMovement(float deltaTime, in RaycastHit hit);
}
public abstract class PlayerMovementStateConfigBase : ScriptableObject
{
    [SerializeField]
    private string _name;
    public string Name => _name;

    protected abstract IPlayerMovementState CreateMovementState();

    public IPlayerMovementState CreateMovementStateInstance(
        PlayerMovementStateMachine machine, 
        MovementInputEventHandler inputHandler)
    {
        IPlayerMovementState newState = CreateMovementState();
        newState.InitializeContext(machine, inputHandler);
        return newState;
    }
}
