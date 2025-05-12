using UnityEngine;

public class EnemyTriggerZone : MonoBehaviour
{
    [SerializeField] private NewEnemyBase[] enemiesCollection;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (enemiesCollection != null && enemiesCollection.Length > 0)
            {
                Debug.LogWarning($"{gameObject.name}: I see Player. Activating mobs!");
                foreach(NewEnemyBase enemy in enemiesCollection)
                {
                    if (enemy != null)
                    {
                        enemy.SetPlayer(other.gameObject.transform);
                        if (enemy.TryGetComponent<BoxCollider>(out BoxCollider localTriggerZone))
                        {
                            if (!localTriggerZone.enabled)
                                localTriggerZone.enabled = true;
                        }
                    }
                }
            }
            else
            {
                Debug.LogError($"{gameObject.name}: collection of enemies is empty!");
            }
        }
    }
}
