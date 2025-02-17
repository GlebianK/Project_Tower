using UnityEngine;

public class NewEnemyLight : NewEnemyBase
{
    private void Update()
    {
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
