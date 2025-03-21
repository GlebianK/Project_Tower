using System.Collections;
using UnityEngine;

public class CombatAnimations : MonoBehaviour
{
    [SerializeField] private float stunnedAnimationTimer = 1.5f;

    private Animator animator;
    private bool isInStunnedState = false;

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

    #region INJURE ANIMATION CALLBACKS FOR LIGHT ENEMY
    public void OnGetHit()
    {
        animator.SetTrigger("isHit");
    }
    public void OnStun()
    {
        if (!isInStunnedState)
            StartCoroutine(StunTimer(stunnedAnimationTimer));
    }

    private IEnumerator StunTimer(float timerValue)
    {
        isInStunnedState = true;
        animator.SetBool("isStunned", true);
        yield return new WaitForSeconds(timerValue);
        animator.SetBool("isStunned", false);
        isInStunnedState = false;
    }
    #endregion
}
