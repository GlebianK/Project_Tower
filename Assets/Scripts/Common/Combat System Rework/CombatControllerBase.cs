using UnityEngine;
using UnityEngine.Events;

public abstract class CombatControllerBase : MonoBehaviour
{
    public UnityEvent AttackStarted;
    public UnityEvent AttackEnded;

    public UnityEvent BlockStarted;
    public UnityEvent BlockEnded;

    protected abstract void Attack();
    protected virtual void Block()
    {
        Debug.Log("You're using base Block() method from CombatControllerBase. Be sure to use the overriden one!");
    }
}
