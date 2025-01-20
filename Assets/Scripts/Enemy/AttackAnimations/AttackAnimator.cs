using UnityEngine;

public class AttackAnimator : MonoBehaviour
{
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    #region
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
