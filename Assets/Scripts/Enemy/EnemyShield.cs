using UnityEngine;

public class EnemyShield : EnemyBase
{

    [SerializeField] private bool isShielded; // TODO: убрать сериализацию

    private void Update()
    {
        EnemyBehaviorCycle();
    }

    private void OnTriggerEnter(Collider other)
    {
        DetectPlayer(other);
    }

    private void OnTriggerExit(Collider other)
    {
        LosePlayer(other);
    }

    protected override void EnemyBehaviorCycle()
    {
        base.EnemyBehaviorCycle();
    }
    
}
