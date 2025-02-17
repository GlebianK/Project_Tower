using UnityEngine;
using UnityEngine.Events;

public class NewBlockBase : ICombatAction
{
    public UnityEvent BlockStarted;
    public UnityEvent BlockEnded;

    private bool canBlock = true;

    private void PerformBlock()
    {
        canBlock = false;
        BlockStarted.Invoke();
    }

    private void CancelBlock()
    {
        BlockEnded.Invoke();
        canBlock = true;
    }

    public void Cancel()
    {
        CancelBlock();
    }

    #region PUBLIC METHODS FROM ICombatAction INTERFACE
    public bool CanPerform() => canBlock;

    public void Perform()
    {
        Debug.LogWarning("Wanna BLOCK!");
        if (CanPerform())
        {
            Debug.LogWarning("BLOCK THE HIT!");
            PerformBlock();
        }
    }
    #endregion
}
