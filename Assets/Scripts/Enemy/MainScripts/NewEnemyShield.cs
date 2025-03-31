using UnityEngine;

public class NewEnemyShield : NewEnemyBase
{
    [SerializeField] private GameObject shieldObject;

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


    public void DropShield()
    {
        if(TryGetComponent<Health>(out Health health))
        {
            Debug.Log($"Current Shield Enemy health: {health.HP}. Half max: {health.MaxHP/2f}");
            if(health.HP > (health.MaxHP/2f))
                return;

            if(shieldObject.activeSelf)
                shieldObject.SetActive(false);
        }        
    }
}
