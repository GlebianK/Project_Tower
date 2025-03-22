using UnityEngine;


public class EnemyLight : EnemyBase
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
