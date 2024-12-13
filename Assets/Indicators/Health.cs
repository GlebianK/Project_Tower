using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] private float Hp = 1f;
    private float StartHp;
    [SerializeField] private UnityEvent DiedGO;
    [SerializeField] private UnityEvent HealedGO;
    [SerializeField] private UnityEvent TookDamageGO;
    private void Start()
    {
        StartHp = Hp;
    }
    public void Heal(float healValue)
    {
        if(Hp <= StartHp)
        {
            Healed();
            Hp += healValue; 
        }
    }
    public void TakeDamage(float damageValue)
    {
        Hp -= damageValue;
        TookDamage();
        if(Hp <= 0f)
        {
            Died();
        }
    }
    private void Died()
    {
        DiedGO.Invoke();
    }
    private void Healed()
    {
        HealedGO.Invoke();
    }
    private void TookDamage()
    {
        TookDamageGO.Invoke();
    }

}
