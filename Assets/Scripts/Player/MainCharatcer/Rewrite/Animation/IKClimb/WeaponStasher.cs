using UnityEngine;

public class WeaponStasher : MonoBehaviour
{
    [SerializeField] private Transform weapon;

    private Vector3 scale;

    private void Awake()
    {
        scale = weapon.localScale;
    }

    public void OnStateChanged(PlayerMovementStateMachine machine, PlayerMovementStateType type)
    {
        if (type == PlayerMovementStateType.Climb)
        {
            weapon.localScale = Vector3.zero;
        }
        else
        {
            weapon.localScale = scale;
        }
    }
}
