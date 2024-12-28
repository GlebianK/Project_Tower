using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public UnityEvent Died;
    public UnityEvent Healed;
    public UnityEvent TookDamage;

    [SerializeField] private float maxHp = 1f;
    [SerializeField] private float hp;

    private void Start()
    {
        hp = maxHp;
    }
    public void Heal(float healValue)
    {
        if(hp <= maxHp)
        {
            hp += healValue;
           // Healed.Invoke();
        }
    }
    public void TakeDamage(float damageValue)
    {
        hp -= damageValue;
      //  TookDamage.Invoke();
        if(hp <= 0f)
        {
            Died.Invoke();
        }
    }



}
