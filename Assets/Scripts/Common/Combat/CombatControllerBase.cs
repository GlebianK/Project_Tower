using UnityEngine;

public abstract class CombatControllerBase : MonoBehaviour
{
    public abstract bool IsAttackAllowed();
    public abstract bool IsBlockAllowed();

    public abstract void Attack();
    public virtual void Block()
    {
        Debug.Log("You're using base Block() method from CombatControllerBase. Be sure to use the overriden one!");
    }
}
