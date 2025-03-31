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
    public UnityEvent TookHeavyDamage;

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
        Debug.Log($"Damage recieved: {damageValue}, remaining HP: {hp}");

        TookDamage.Invoke();

        if (hp <= 0f)
        {
            Debug.LogWarning($"{gameObject.name} died!");
            Died.Invoke();
        }
    }

    public void TakeDamage(float damageValue, NewAttackBase.AttackType attackType)
    {
        hp -= damageValue;
        Debug.Log($"Damage: {damageValue}, remaining HP: {hp}");

        if (attackType == NewAttackBase.AttackType.light)
        {
            Debug.LogWarning($"Health: {gameObject} got light hit!");
            TookDamage.Invoke();
        }
        else if (attackType == NewAttackBase.AttackType.heavy)
        {
            Debug.LogWarning($"Health: {gameObject} got heavy hit!");
            TookHeavyDamage.Invoke();
        }

        if(hp <= 0f)
        {
            Died.Invoke();
        }
    }

    public void ActivateDamageReductionByBlock()
    {
        currentDamageReductionCoef = damageReductionCoef;
    }

    public void DeactivateDamageReductionByBlock()
    {
        currentDamageReductionCoef = 0f;
    }
}
