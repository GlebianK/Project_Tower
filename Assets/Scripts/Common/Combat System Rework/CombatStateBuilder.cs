using UnityEngine;

public interface ICombatStateBuilder
{
    public virtual ICombatStateConfig CreateStateConfig()
    {
        return null; // TODO: сделать нормально!
    }
}
