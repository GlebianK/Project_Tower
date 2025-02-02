using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombatSystemController : MonoBehaviour
{
    [SerializeField] private float attackTypeTimerThreshold = 0.35f;
    [SerializeField] private GameObject attackLightGO; // delete on rework
    [SerializeField] private GameObject attackHeavyGO; // delete on rework

    private float attackPreparationTimer;
    private bool timerIsCounting;

    [SerializeField] private AttackPlayerLight attackPlayerLight; // delete on rework
    [SerializeField] private AttackPlayerHeavy attackPlayerHeavy; // delete on rework

    private bool isAttackBlockedByClimb;

    private void Start()
    {
        InitializeCombatController();
    }

    private void InitializeCombatController()
    {
        attackPreparationTimer = 0f;
        timerIsCounting = false;
        isAttackBlockedByClimb = false;

        (attackPlayerLight, attackPlayerHeavy) = TryGetAttackComponents(attackLightGO, attackHeavyGO); // delete on rework
        attackLightGO.SetActive(false); // delete on rework
        attackHeavyGO.SetActive(false); // delete on rework
    }

    // delete on rework
    private (AttackPlayerLight, AttackPlayerHeavy) TryGetAttackComponents(GameObject attackGO1, GameObject attackGO2)
    {
        if (attackGO1.TryGetComponent<AttackPlayerLight>(out AttackPlayerLight temp_light)
            && attackGO2.TryGetComponent<AttackPlayerHeavy>(out AttackPlayerHeavy temp_heavy))
        {
            return (temp_light, temp_heavy);
        }
        else
        {
            throw new Exception("Cpmbat Controller: SOMETHING WENT WRONG WHILE TRYING TO RETRIEVE ATTACK COMPONENTS !!!");
        }
        
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

    #region INPUT CALLBACKS
    public void OnAttackPrepare(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if(attackPlayerLight.CanAttack() && attackPlayerHeavy.CanAttack())
                StartCoroutine(AttackPreparationTimer());
        }
    }

    public void OnAttackInitiate(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            timerIsCounting = false;
            if (isAttackBlockedByClimb) return;

            if (attackPreparationTimer <= attackTypeTimerThreshold)
            {
                attackLightGO.SetActive(true);
                attackPlayerLight.TryAttack();
            }
            else
            {
                attackHeavyGO.SetActive(true);
                attackPlayerHeavy.TryAttack();
            }
        }
    }

    public void OnBlock(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.LogWarning("Player block is not implemented !!!");
            throw new NotImplementedException("PlayerCombatController.cs -> OnBlock - IMPLEMENT!");
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
