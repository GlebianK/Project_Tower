using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

[System.Serializable]
public struct StateToNameDescriptor
{
    public PlayerMovementStateType type;
    public string name;
}

public class PlayerAnimationLinker : MonoBehaviour
{
    [SerializeField] private PlayerMovementStateMachine movementController;
    [SerializeField] private PlayerAnimationSystem animationSystem;
    [SerializeField] private string defaultStateName;
    [SerializeField] private List<StateToNameDescriptor> states;

    [SerializeField] private string attackLightAnimState;
    [SerializeField] private string attackHeavyAnimState;

    private bool isAttacking = false;
    private string movementAnimStateName = "";

    private void OnEnable()
    {
        movementController.StateChanged.AddListener(OnStateChanged);
    }

    private void OnDisable()
    {
        movementController.StateChanged.RemoveListener(OnStateChanged);
    }

    private void OnStateChanged(PlayerMovementStateMachine machine, PlayerMovementStateType stateType)
    {
        IEnumerable<StateToNameDescriptor> state = states.Where(pair => pair.type == stateType);
        string stateName;
        if (state.Count() == 0)
            stateName = defaultStateName;
        else
            stateName = state.First().name;

        movementAnimStateName = stateName;

        if (!isAttacking)
        {
            animationSystem.SetState(movementAnimStateName);
        }
    }

    public void OnAttackStarted(bool isHeavy)
    {
        string stateName = isHeavy ? attackHeavyAnimState : attackLightAnimState;
        animationSystem.SetState(stateName);

        isAttacking = true;
    }

    public void OnAttackEnded()
    {
        isAttacking = false;

        //animationSystem.SetState(movementAnimStateName);
    }
}
