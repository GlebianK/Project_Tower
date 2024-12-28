using UnityEngine;

public class MoveAnimator : MonoBehaviour
{
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    #region
    public void MoveStart()
    {
        animator.SetBool("Move", true);
    }
    public void MoveEnded()
    {
        animator.SetBool("Move",false);
    }
    #endregion
}
