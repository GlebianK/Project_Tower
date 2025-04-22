using UnityEngine;

public class NewEnemyLight : NewEnemyBase
{
    [SerializeField] private CombatAnimations combatAnimationsComponent;

    protected void Awake()
    {
        base.Awake();

        rewardResolveType = EnemyType.Light;
    }

    private void Update()
    {
        if (isAlive && !combatAnimationsComponent.IsInStunnedState)
        {
            /*
            if (agent.isStopped)
                agent.isStopped = false;
            */
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
            agent.SetDestination(gameObject.transform.position);
            agent.speed = 0;
            agent.angularSpeed = 0;
            agent.isStopped = true;
            Debug.Log("New Enemy Light: STUNNED!");
        }
    }
}
