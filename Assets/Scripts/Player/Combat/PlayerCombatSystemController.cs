using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerCombatSystemController : MonoBehaviour
{
    private float attackPreparationTimer;
    private bool timerIsCounting;

    public bool CanAttack { get; set; } // åñòü â êëàññå àòàêè, óáðàòü!!!


    private void Start()
    {
        CanAttack = true;
        attackPreparationTimer = 0f;
        timerIsCounting = false;
    }

    private IEnumerator AttackPreparationTimer()
    {
        Debug.LogWarning("AttackPreparationTimer coroutine started!");
        if (CanAttack)
        {
            attackPreparationTimer = 0f;
            timerIsCounting = true;

            while (timerIsCounting)
            {
                attackPreparationTimer += Time.deltaTime;
                Debug.Log($"TIMER! value = {attackPreparationTimer}");
                yield return null;
            }
            Debug.Log($"timer ends, value = {attackPreparationTimer}");
        }
        Debug.LogWarning("End of coroutine");
        yield return null;
    }

    #region INPUT CALLBACKS
    public void OnAttackPrepare(InputAction.CallbackContext context)
    {

        CanAttack = true; // ÓÁÐÀÒÜ ÝÒÎ ÍÀÕÓÉ Â ÐÅËÈÇÍÎÉ ÂÅÐÑÈÈ, ×ÈÑÒÎ ÂÐÅÌÅÍÍÀß ÇÀÒÛ×ÊÀ !!!!!!!!!!!!!!!

        if (context.performed)
        {
            Debug.LogWarning("Ready to attack!!!");
            StartCoroutine(AttackPreparationTimer());
            //TODO: Invoke attack preparation event (run the animation?)
        }
    }

    public void OnAttackInitiate(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.LogWarning("ATTACK !!!");
            CanAttack = false;
            timerIsCounting = false;
            //TODO: Ivoke attack event from attack system (run the animation?)
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
}
