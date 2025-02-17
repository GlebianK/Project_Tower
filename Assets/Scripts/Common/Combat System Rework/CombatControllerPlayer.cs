using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class CombatControllerPlayer : CombatControllerBase
{
    [SerializeField] private float attackTypeTimerThreshold = 0.35f;
    [SerializeField] private NewAttackPlayer[] attacksPlayer;
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

    /*
    private NewAttackPlayer GetAttackByType(NewAttackPlayer[] attack)
    {

    }
    */

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
        throw new NotImplementedException("No attack for player!");
    }

    public override void Block()
    {
        throw new NotImplementedException("No block for player!");
    }

    #region INPUT CALLBACKS
    public void OnAttackPrepare(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (IsAttackAllowed())
                StartCoroutine(AttackPreparationTimer());
            prepareAttack.Invoke();
        }
    }

    public void OnAttackInitiate(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            timerIsCounting = false;
            if (isAttackBlockedByClimb) 
                return;

            if (attackPreparationTimer <= attackTypeTimerThreshold)
            {
                //attackLightGO.SetActive(true);
                //attackPlayerLight.TryAttack();
            }
            else
            {
                //attackHeavyGO.SetActive(true);
                //attackPlayerHeavy.TryAttack();
            }
            
        }
    }

    public void OnBlockStart(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (IsBlockAllowed())
                blockPlayer.Perform();
        }
    }

    public void OnBlockEnd(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
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
