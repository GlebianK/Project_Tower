using UnityEngine;

public class CombatMovementBlocker : MonoBehaviour
{
    [SerializeField] private PlayerMovementStateMachine movementController;

    public void OnAttackStarted()
    {
        movementController.BlockMovementState(PlayerMovementStateType.Sprint);
        movementController.BlockMovementState(PlayerMovementStateType.Slide);
        movementController.BlockMovementState(PlayerMovementStateType.Climb);
    }
    public void OnAttackEnded()
    {
        movementController.UnblockMovementState(PlayerMovementStateType.Sprint);
        movementController.UnblockMovementState(PlayerMovementStateType.Slide);
        movementController.UnblockMovementState(PlayerMovementStateType.Climb);
    }
}
