using System.Collections;
using UnityEngine;

public class CombatControllerEnemy : CombatControllerBase
{
    [SerializeField] private NewAttackEnemy attackEnemy;
    [SerializeField] private NewBlockEnemy blockEnemy;

    [SerializeField] private float minAttackCooldown = 0.15f;
    [SerializeField] private float maxAttackCooldown = 0.75f;

    [SerializeField] private float afterHitRecoveryTimer = 1.75f;

    [SerializeField] private AudioSource attackAudioSource;

    private bool attackIsOnCooldown = false;
    private bool isRecoveringAfterHit;

    public override bool IsAttackAllowed() => attackEnemy.CanPerform();
    public override bool IsBlockAllowed() => blockEnemy.CanPerform();

    public bool IsRecoveringAfterHit() => isRecoveringAfterHit;

    private void Awake()
    {
        attackIsOnCooldown = false;
        isRecoveringAfterHit = false;
    }

    public override void Attack()
    {
        if (!attackIsOnCooldown && !isRecoveringAfterHit)
        {
            attackEnemy.Perform();
            StartCoroutine(AttackCooldown());
        }
    }

    public override void Block()
    {
        blockEnemy.Perform();
    }

    private IEnumerator AttackCooldown()
    {
        attackIsOnCooldown = true;
        float cd_timer = attackEnemy.DurationOfAttack + Random.Range(minAttackCooldown, maxAttackCooldown);
        Debug.Log($"Enemy {gameObject.name} attack cd: {cd_timer}");
        yield return new WaitForSeconds(cd_timer);
        attackIsOnCooldown = false;
    }

    // Колбэк на аборт атаки при получении урона (подписан на TookDamage и TookHeavyDamage компонента Health) 
    public void OnAttackAborted()
    {
        attackEnemy.attackIsAborted = true;
    }

    // Коллбэк на получение удара (чтобы не было нанесения урона игроку при анимации отшатывания)
    public void OnGetHit()
    {
        StartCoroutine(RecoverAfterHit(afterHitRecoveryTimer));
        StopCoroutine(AttackCooldown());
        attackIsOnCooldown = false;
    }

    private IEnumerator RecoverAfterHit(float timerValue)
    {
        isRecoveringAfterHit = true;
        Debug.Log("Combat Animations: WAS HIT! Recovering...");
        yield return new WaitForSeconds(timerValue);
        isRecoveringAfterHit = false;
        Debug.Log("Combat Animations: RECOVERED!");
    }
}
