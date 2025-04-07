using System.Collections;
using UnityEngine;

public class CombatControllerEnemy : CombatControllerBase
{
    [SerializeField] private NewAttackEnemy attackEnemy;
    [SerializeField] private NewBlockEnemy blockEnemy;

    [SerializeField] private float minAttackCooldown = 0.15f;
    [SerializeField] private float maxAttackCooldown = 0.75f;

    private bool attackIsOnCooldown = false;

    public override bool IsAttackAllowed() => attackEnemy.CanPerform();
    public override bool IsBlockAllowed() => blockEnemy.CanPerform();

    private void Awake()
    {
        attackIsOnCooldown = false;
    }

    public override void Attack()
    {
        if (!attackIsOnCooldown)
        {
            attackEnemy.Perform();
            StartCoroutine(AttackCooldown());
        }
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

    private IEnumerator AttackCooldown()
    {
        attackIsOnCooldown = true;
        float cd_timer = Random.Range(minAttackCooldown, maxAttackCooldown);
        Debug.Log($"Enemy {gameObject.name} attack cd: {cd_timer}");
        yield return new WaitForSeconds(cd_timer);
        attackIsOnCooldown = false;
    }
}
