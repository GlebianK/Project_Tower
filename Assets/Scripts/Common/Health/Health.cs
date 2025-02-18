using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHp = 1f;
    [Range(0f, 1f), SerializeField] private float damageReductionCoef = 0.25f;

    private float hp;
    private float currentDamageReductionCoef;

    public float MaxHP => maxHp;
    public float HP => hp;
    public float DamageReductionCoef => currentDamageReductionCoef;

    public UnityEvent Died;
    public UnityEvent Healed;
    public UnityEvent TookDamage;

    private void Awake()
    {
        hp = maxHp;
        currentDamageReductionCoef = 0f;
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

    public void ActivateDamageReduction()
    {
        currentDamageReductionCoef = damageReductionCoef;
    }

    public void DeactivateDamageReduction()
    {
        currentDamageReductionCoef = 0f;
    }
}
