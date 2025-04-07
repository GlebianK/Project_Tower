using UnityEngine;

public class NewEnemyLight : NewEnemyBase
{
    [SerializeField] private CombatAnimations combatAnimationsComponent;

    private void Update()
    {
        if (!combatAnimationsComponent.IsInStunnedState)
        {
            agent.isStopped = false;
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
