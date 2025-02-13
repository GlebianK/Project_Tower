using UnityEngine;

public class CombatControllerEnemy : CombatControllerBase
{
    protected override void Attack()
    {
        throw new System.NotImplementedException("No attack for enemy!");
    }

    protected override void Block()
    {
        throw new System.NotImplementedException("No block for enemy!");
    }
}
