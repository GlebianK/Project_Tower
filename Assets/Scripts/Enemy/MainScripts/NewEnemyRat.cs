using UnityEngine;

public class NewEnemyRat : NewEnemyBase
{
    protected void Awake()
    {
        base.Awake();

        rewardResolveType = EnemyType.Rat;
    }
    private void Update()
    {
        if (isAlive)
            EnemyBehaviourCycle();
    }

    private void OnTriggerEnter(Collider other)
    {
        DetectPlayer(other);
    }

    private void OnTriggerExit(Collider other)
    {
        LosePlayer(other);
    }
}
