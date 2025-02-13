using UnityEngine;

public class CombatControllerPlayer : CombatControllerBase
{
    protected override void Attack()
    {
        throw new System.NotImplementedException("No attack for player!");
    }

    protected override void Block()
    {
        throw new System.NotImplementedException("No block for player!");
    }
}
