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
    protected PlayerMovementStateMachine movementController { get; private set; }
    protected MovementInputEventHandler inputController { get; private set; }
    protected PlayerMovementStateConfig config { get; private set; }
    public PlayerMovementStateBase(PlayerMovementStateConfig config)
    {
        this.config = config;
    }
    public void InitializeContext(PlayerMovementStateMachine machine, MovementInputEventHandler inputHandler)
    {
        movementController = machine;
        inputController = inputHandler;
    }

    public abstract void OnStateActivated(IPlayerMovementState prevState);
    public abstract void OnStateDeactivated(IPlayerMovementState nextState);

    // возвращает true, если необходимо перейти в другое состояние
    public abstract bool MakeTransitions(float deltaTime);
    public void UpdateMovementVelocity(float deltaTime)
    {
        if (MakeTransitions(deltaTime))
            return;

        movementController.CharacterVelocity = ComputeVelocity(deltaTime);
    }
    protected abstract Vector3 ComputeVelocity(float deltaTime);
    public abstract void HandleObstacleAfterMovement(float deltaTime, in RaycastHit hit);
}
