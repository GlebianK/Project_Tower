using UnityEngine;

public class CombatControllerEnemy : CombatControllerBase
{
    [SerializeField] private NewAttackEnemy attackEnemy;
    [SerializeField] private NewBlockEnemy blockEnemy;

    public override bool IsAttackAllowed() => attackEnemy.CanPerform();
    public override bool IsBlockAllowed() => blockEnemy.CanPerform();

    public override void Attack()
    {
        attackEnemy.Perform();
    }

    public override void Block()
    {
        blockEnemy.Perform();
    }
     // Колбэк на аборт атаки при получении урона (подписан на TookDamage и TookHeavyDamage компонента Health) 
    public void OnAttackAborted()
    {
        attackEnemy.attackIsAborted = true;
    }
}
