using UnityEngine;
using UnityEngine.Events;

public class MotionEnemy: MonoBehaviour
{
    public UnityEvent MoveStarted;
    public UnityEvent FollowStarted;
    public UnityEvent MoveEnded;
    public UnityEvent FollowEnded;

    public void StartMovingAnimations() => MoveStarted.Invoke();
    public void StopMovingAnimations() => MoveEnded.Invoke();
    public void StartFollowAnimations() => FollowStarted.Invoke();
    public void StopFollowAnimations() => FollowEnded.Invoke();
}
