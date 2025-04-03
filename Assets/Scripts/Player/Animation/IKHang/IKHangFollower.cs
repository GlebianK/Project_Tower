using UnityEngine;

public class IKHangFollower : MonoBehaviour
{
    [SerializeField] private IKHangRail controller;
    [SerializeField] private Transform startSnapPoint;
    [SerializeField] private Transform leftPoint;
    [SerializeField] private Transform rightPoint;
    [SerializeField] private float speed = 5;
    [SerializeField] private float angularSpeed = 60.0f;
    [SerializeField] private float wakeDistanceThreshold = 1.5f;
    [SerializeField] private float sleepAngularThreshold = 1.0f;
    [SerializeField] private float sleepDistanceThreshold = 0.01f;

    public Transform LeftPoint => leftPoint;
    public Transform RightPoint => rightPoint;

    private bool isSnapping = false;

    private void OnEnable()
    {
        transform.position = startSnapPoint.position;
        transform.rotation = startSnapPoint.rotation;
        isSnapping = false;
    }

    private void Update()
    {
        Vector3 targetPos = Vector3.Lerp(LeftPoint.transform.position, 
            RightPoint.transform.position, 
            controller.LeftRight);
        Quaternion targetRot = Quaternion.Slerp(LeftPoint.transform.rotation,
            RightPoint.transform.rotation,
            controller.LeftRight);

        if (controller.LeftRight == 0.5)
        {
            targetPos = startSnapPoint.position;
            targetRot = startSnapPoint.rotation;

            isSnapping = true;
        }

        if (isSnapping)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, angularSpeed * Time.deltaTime);
            
            float distance = (targetPos - transform.position).magnitude;
            float angle = Quaternion.Angle(targetRot, transform.rotation);

            if (distance < sleepDistanceThreshold && angle < sleepAngularThreshold)
            {
                isSnapping = false;
            }
        }
        else
        {
            float distance = (targetPos - transform.position).magnitude;
            if (distance > wakeDistanceThreshold)
            {
                isSnapping = true;
            }
        }
    }
}
