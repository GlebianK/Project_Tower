using UnityEngine;

public class Enemy : MonoBehaviour
{
    public void EnemyDied()
    {
        Destroy(gameObject);
    }
}
