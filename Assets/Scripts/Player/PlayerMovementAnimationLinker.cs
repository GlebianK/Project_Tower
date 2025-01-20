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

public class PlayerMovementAnimationLinker : MonoBehaviour
{
    [SerializeField] private PlayerMovementStateMachine movementController;
    [SerializeField] private PlayerAnimationSystem animationSystem;
    [SerializeField] private string defaultStateName;
    [SerializeField] private List<StateToNameDescriptor> states;

    private void OnEnable()
    {
        movementController.StateChanged.AddListener(OnStateChanged);
    }

    private void OnDisable()
    {
        movementController.StateChanged.RemoveListener(OnStateChanged);
    }

    private void OnStateChanged(PlayerMovementStateMachine _, PlayerMovementStateType stateType)
    {
        IEnumerable<StateToNameDescriptor> state = states.Where(pair => pair.type == stateType);
        string stateName;
        if (state.Count() == 0)
            stateName = defaultStateName;
        else
            stateName = state.First().name;

        animationSystem.SetState(stateName);
    }
}
