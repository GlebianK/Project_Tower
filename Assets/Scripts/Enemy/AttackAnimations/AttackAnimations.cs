using UnityEngine;

public class AttackAnimations : MonoBehaviour
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
