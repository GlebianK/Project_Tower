using UnityEngine;

public class CombatMovementBlocker : MonoBehaviour
{
    [SerializeField] private PlayerMovementStateMachine movementController;

    public void OnAttackStarted()
    {
        movementController.BlockMovementState(PlayerMovementStateType.Sprint);
        movementController.BlockMovementState(PlayerMovementStateType.Slide);
        movementController.BlockMovementState(PlayerMovementStateType.Climb);
        movementController.BlockMovementState(PlayerMovementStateType.Hang);
    }
    public void OnAttackEnded()
    {
        movementController.UnblockMovementState(PlayerMovementStateType.Sprint);
        movementController.UnblockMovementState(PlayerMovementStateType.Slide);
        movementController.UnblockMovementState(PlayerMovementStateType.Climb);
        movementController.UnblockMovementState(PlayerMovementStateType.Hang);
    }
}
