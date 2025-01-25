using UnityEngine;

public class MotionAnimation: MonoBehaviour
{
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    #region
    public void MovingStarted()
    {
        animator.SetBool("canMove", true);
    }
    public void MovingEnded()
    {
        animator.SetBool("canMove",false);
    }
    #endregion
}
