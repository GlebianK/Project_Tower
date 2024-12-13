using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] private float _maxHp = 1f;
    private float _hp;
    public UnityEvent Died;
    public UnityEvent Healed;
    public UnityEvent TookDamage;
    private void Start()
    {
        _hp = _maxHp;
    }
    public void Heal(float healValue)
    {
        if(_hp <= _maxHp)
        {
            _hp += healValue;
            Healed.Invoke();
        }
    }
    public void TakeDamage(float damageValue)
    {
        _hp -= damageValue;
        TookDamage.Invoke();
        if(_hp <= 0f)
        {
            Died.Invoke();
        }
    }



}
