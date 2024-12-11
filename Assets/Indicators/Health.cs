using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float Hp = 1f;

    public void Heal(float healValue)
    {
        Hp += healValue;
    }
    public void TakeDamage(float damageValue)
    {
        Hp -= damageValue;
        if(Hp <= 0f)
        {
            Died();
        }
    }
    private void Died()
    {
        Debug.Log("Died");
    }

}
