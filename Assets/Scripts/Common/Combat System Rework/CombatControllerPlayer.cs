using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class CombatControllerPlayer : CombatControllerBase
{
    [SerializeField] private Health playerHealthComponent;
    [SerializeField] private float attackTypeTimerThreshold = 0.35f;

    [Tooltip("Add light and heavy attacks in the array. Start with the light attack")]
    [SerializeField] private NewAttackPlayer[] attacksPlayer;

    [Tooltip("Player block settings")]
    [SerializeField] private NewBlockPlayer blockPlayer;

    public UnityEvent prepareAttack;

    private float attackPreparationTimer;
    private bool timerIsCounting;
    private bool isAttackBlockedByClimb;

    #region ABSTRACT BASE METHODS OVERRIDE
    public override bool IsBlockAllowed() => blockPlayer.CanPerform();
    public override bool IsAttackAllowed()
    {
        if (attacksPlayer != null && attacksPlayer.Length > 0)
        {
            bool temp = true;
            foreach (NewAttackPlayer attackPlayer in attacksPlayer)
                temp &= attackPlayer.CanPerform();
            return temp;
        }
        else
            throw new Exception("а какова ху€ атак нихватаит?!1!7");
    }
    #endregion

    private void Start()
    {
        InitializeCombatController();
    }

    private void InitializeCombatController()
    {
        attackPreparationTimer = 0f;
        timerIsCounting = false;
        isAttackBlockedByClimb = false;
    }

    private IEnumerator AttackPreparationTimer()
    {
        attackPreparationTimer = 0f;
        timerIsCounting = true;

        while (timerIsCounting && !isAttackBlockedByClimb)
        {
            attackPreparationTimer += Time.deltaTime;
            yield return null;
        }

        yield return null;
    }

    public override void Attack()
    {
        NewAttackPlayer currentAttack;

        if (attackPreparationTimer <= attackTypeTimerThreshold)
        {
            Debug.Log("Light attack");
            currentAttack = attacksPlayer[0];
        }
        else
        {
            Debug.Log("HeavyAttack");
            currentAttack = attacksPlayer[1];
        }

        if (currentAttack.CanPerform())
        {
            Debug.LogWarning("CC_Player: ATTACK !!!!!!");
            currentAttack.Perform();
        }
           
    }

    public override void Block()
    {
        if (IsBlockAllowed())
        {
            blockPlayer.Perform();
        }
    }

    #region INPUT CALLBACKS
    public void OnAttackPrepare(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!IsBlockAllowed())
                return;

            if (IsAttackAllowed())
            {
                StartCoroutine(AttackPreparationTimer());
                prepareAttack.Invoke();
            }
        }
    }

    public void OnAttackInitiate(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            timerIsCounting = false;
            if (isAttackBlockedByClimb) 
                return;

            if (!IsBlockAllowed())
                return;

            Attack();            
        }
    }

    public void OnBlockInitiate(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!IsAttackAllowed())
                return;

            Debug.LogWarning("Block initiated!");
            playerHealthComponent.ActivateDamageReduction();
            Block();

        }
    }

    public void OnBlockCancel(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.LogWarning("Block stopped!");
            playerHealthComponent.DeactivateDamageReduction();
            blockPlayer.Cancel();
        }
    }
    #endregion

    public void OnMovementStateChanged(PlayerMovementStateMachine machine, PlayerMovementStateType type)
    {
        if (type == PlayerMovementStateType.Climb)
        {
            isAttackBlockedByClimb = true;
        }
        else
        {
            isAttackBlockedByClimb = false;
        }
    }
}
