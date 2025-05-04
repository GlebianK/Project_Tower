using UnityEngine;

public class NewEnemyLight : NewEnemyBase
{
    [SerializeField] private CombatAnimations combatAnimationsComponent;

    protected new void Awake()
    {
        base.Awake();

        rewardResolveType = EnemyType.Light;
    }

    private void Update()
    {
        if (isAlive && !combatAnimationsComponent.IsInStunnedState)
        {
            EnemyBehaviourCycle();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        DetectPlayer(other);
    }

    private void OnTriggerExit(Collider other)
    {
        LosePlayer(other);
    }

    public void OnStun()
    {
        if(!combatAnimationsComponent.IsInStunnedState)
        {
            agent.ResetPath();
            StopEnemyMovement();
            Debug.Log("New Enemy Light: STUNNED!");
        }
    }
}
