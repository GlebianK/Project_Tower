using UnityEngine;

public interface ICombatAction
{
    public bool CanPerform(); // Проверка возможности совершения действия
    public void Perform(); // Совершение действия
}
