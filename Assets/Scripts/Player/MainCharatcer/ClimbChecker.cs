using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ClimbChecker : MonoBehaviour
{
    [SerializeField]
    private LayerMask _groundLayer;
    [SerializeField]
    private Vector3 _groundCheckVector;
    [SerializeField]
    private AnimationCurve _clibCurve;
    [SerializeField]
    private float _maxClimbTime;
    [Header("Animation")]
    [SerializeField]
    private Transform _climbIKPointParentTransform;
    [SerializeField]
    private float _handDetachTimeThreshold = 2.0f;

    public RaycastHit GetEndClimbPoint(CharacterController cc)
    {
        Vector3 upperControllerPoint = new Vector3(cc.transform.position.x, transform.position.y, cc.transform.position.z);
        
        //check if capsule cannot get up
        if (Physics.Raycast(
            cc.transform.position,
            (Vector3.up * cc.height + cc.center).normalized,
            (upperControllerPoint - cc.transform.position).magnitude,
            _groundLayer,
            QueryTriggerInteraction.Ignore))
        {
            return new RaycastHit();
        }

        //check for point of climbing
        RaycastHit raycastHit;
        if (!Physics.Raycast(
            transform.position,
            _groundCheckVector.normalized,
            out raycastHit,
            _groundCheckVector.magnitude,
            _groundLayer,
            QueryTriggerInteraction.Ignore
            ))
        {
            return raycastHit;
        }

        //check if any colliders messes up climbing
        //Debug.DrawLine(raycastHit.point + Vector3.up * cc.skinWidth, raycastHit.point + Vector3.up * (cc.skinWidth + cc.height), Color.red);
        //if (Physics.CheckCapsule(raycastHit.point + Vector3.up * cc.skinWidth, 
        //    raycastHit.point + Vector3.up * (cc.skinWidth + cc.height),
        //    cc.radius, _groundLayer, QueryTriggerInteraction.Ignore))
        //{
        //    return new RaycastHit();
        //}

        return raycastHit;
    }

    public IEnumerator ClimbOverTime(PlayerMovementController moveController, RaycastHit endPointHit, Vector3 wallNormal)
    {
        moveController.OnClimbStarted.Invoke();
        Vector3 endPoint = endPointHit.point;

        CharacterController cc = moveController.GetComponent<CharacterController>();

        _climbIKPointParentTransform.position = endPoint;
        //Vector3 positionToLookAt = endPoint;
        //positionToLookAt += wallNormal;
        //_climbIKPointParentTransform.LookAt(positionToLookAt);
        _climbIKPointParentTransform.rotation = Quaternion.LookRotation(wallNormal, endPointHit.normal);

        cc.enabled = false;

        Vector3 startPosition = cc.transform.position;
        Vector3 totalMovementVector = endPoint - startPosition;

        Vector3 horizontalVelocity = totalMovementVector;
        horizontalVelocity.y = 0;

        float startTimeMinMaxed = 1 - totalMovementVector.magnitude / (transform.position - cc.transform.position).magnitude;

        float startTime = startTimeMinMaxed * _maxClimbTime;
        float currentTime = startTime;

        bool detachCalled = false;

        while (currentTime < _maxClimbTime)
        {
            Vector3 currentPos = startPosition + horizontalVelocity * currentTime / _maxClimbTime;
            float upPosition = endPoint.y + (startPosition - endPoint).y * (1 - _clibCurve.Evaluate((currentTime - startTime) / (_maxClimbTime - startTime)));
            //float upPosition = startPosition.y - _groundCheckVector.y * _clibCurve.Evaluate(currentTime / _maxClimbTime);

            currentPos.y = upPosition;
            cc.transform.position = currentPos;
            currentTime += Time.deltaTime;
            if (currentTime > _handDetachTimeThreshold && !detachCalled)
            {
                detachCalled = true;
                moveController.OnClimbHandDetach.Invoke();
            }
            yield return null;
        }

        cc.transform.position = endPoint;
        cc.enabled = true;
        if (!detachCalled)
        {
            detachCalled = true;
            moveController.OnClimbHandDetach.Invoke();
        }
        moveController.OnClimbEnded.Invoke();
    }

    private void OnDrawGizmosSelected()
    {
        Debug.DrawLine(transform.position, transform.position + _groundCheckVector, Color.blue, .0f, true);
    }
}
