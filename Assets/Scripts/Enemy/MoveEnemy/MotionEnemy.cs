using UnityEngine;
using UnityEngine.Events;

public class MotionEnemy: MonoBehaviour
{
    public UnityEvent MoveStarted;
    public UnityEvent MoveEnded;

    public void StartMovingAnimations() => MoveStarted.Invoke();
    public void StopMovingAnimations() => MoveEnded.Invoke();
}
