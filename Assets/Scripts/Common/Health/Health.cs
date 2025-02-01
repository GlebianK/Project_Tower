using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public UnityEvent Died;
    public UnityEvent Healed;
    public UnityEvent TookDamage;

    [SerializeField] private float maxHp = 1f;
    private float hp;

    private void Start()
    {
        hp = maxHp;
    }
    public void Heal(float healValue)
    {
        if(hp <= maxHp)
        {
            hp += healValue;
            if (hp > maxHp)
            {
                hp = maxHp;
                Healed.Invoke();
            }
            else
            {
                Healed.Invoke();
            }
        }

    }
    public void TakeDamage(float damageValue)
    {
        hp -= damageValue;
        Debug.Log(hp);
        TookDamage.Invoke();
        if(hp <= 0f)
        {
            Died.Invoke();
        }
    }



}
