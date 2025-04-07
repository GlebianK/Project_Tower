using System.Collections;
using UnityEngine;

public class CombatAnimations : MonoBehaviour
{
    [SerializeField] private float stunnedAnimationTimer = 1.5f;

    private Animator animator;
    private bool isInStunnedState = false;

    public bool IsInStunnedState => isInStunnedState;

    private void Start()
    {
        animator = GetComponent<Animator>();
        isInStunnedState = false;
    }

    #region ATTACK ANIMATION CALLBACKS
    public void OnAttackStarted()
    {
        animator.SetBool("OnAttack",true);
    }
    public void OnAttackEnded()
    {
        animator.SetBool("OnAttack", false);
    }
    #endregion

    // COMMON CALLBACK FOR ALL ENEMIES
    public void OnGetHit()
    {
        animator.SetTrigger("isHit");
    }

    #region INJURE ANIMATION CALLBACKS FOR LIGHT ENEMY
    public void OnStun()
    {
        if (!isInStunnedState)
            StartCoroutine(StunTimer(stunnedAnimationTimer));
    }

    private IEnumerator StunTimer(float timerValue)
    {
        isInStunnedState = true;
        Debug.Log("Combat Animations: STUNNED!");
        animator.SetBool("isStunned", true);
        yield return new WaitForSeconds(timerValue);
        animator.SetBool("isStunned", false);
        isInStunnedState = false;
        Debug.Log("Combat Animations: UNSTUNNED!");
    }
    #endregion
}
