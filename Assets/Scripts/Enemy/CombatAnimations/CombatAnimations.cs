using UnityEngine;

public class CombatAnimations : MonoBehaviour
{
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
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
}
