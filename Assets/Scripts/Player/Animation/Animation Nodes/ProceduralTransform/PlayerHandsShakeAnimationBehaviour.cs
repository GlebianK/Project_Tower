using UnityEngine;
using UnityEngine.Playables;

// A behaviour that is attached to a playable
public class PlayerHandsShakeAnimationBehaviour : ProceduralTransformAnimationBehaviour
{
    public PlayerMovementStateMachine movementController;
    public Vector3 directionMultiplier;

    private Vector3 walkAnimLagPos = Vector3.zero;
    private float lastFrameUp = 0;

    public override void PrepareFrame(Playable playable, FrameData info)
    {
        Vector3 xzVelocity = movementController.CharacterVelocity;
        float velocity = xzVelocity.magnitude;
        float alpha = (velocity / movementController.Properties.WalkMaxSpeed);
        if (!movementController.GroundCheck())
        {
            alpha = 0;
        }
        float forward = Vector3.Dot(movementController.CharacterVelocity, movementController.transform.forward);
        float up = Vector3.Dot(movementController.CharacterVelocity, movementController.transform.up);
        float right = Vector3.Dot(movementController.CharacterVelocity, movementController.transform.right);

        forward = forward / -movementController.Properties.WalkMaxSpeed * directionMultiplier.z;
        right = right / -movementController.Properties.WalkMaxSpeed * directionMultiplier.x;
        up = Mathf.Lerp(lastFrameUp, -up / movementController.Properties.JumpVelocity, 0.3f) * directionMultiplier.y;
        lastFrameUp = up;

        Vector3 lagPos = new Vector3(right, up, forward) * 0.02f;
        lagPos = Vector3.ClampMagnitude(lagPos, 0.04f);
        walkAnimLagPos = Vector3.MoveTowards(walkAnimLagPos, lagPos, Time.deltaTime * 0.1f);

        //resultPositionOffset = walkAnimLagPos * 0.1f;
        resultPositionOffset = Vector3.zero;
        resultRotationOffset = Quaternion.identity;
        resultScaleOffset = Vector3.zero;

        time += Time.deltaTime * info.effectiveSpeed;

        resultPositionOffset += GetPropertiesOffsetByTime(proceduralPosition, time) * alpha;
        resultRotationOffset = Quaternion.Slerp(Quaternion.identity,
            Quaternion.Euler(GetPropertiesOffsetByTime(proceduralRotation, time)) * resultRotationOffset,
            alpha);
        resultScaleOffset += GetPropertiesOffsetByTime(proceduralScale, time) * alpha;

        resultPositionOffset += targetLocalPositionOffset;
        resultRotationOffset = targetLocalRotationOffset * resultRotationOffset;
        resultScaleOffset += targetLocalScaleOffset;

        resultPositionOffset *= info.weight;
        resultRotationOffset = Quaternion.Slerp(Quaternion.identity, resultRotationOffset, info.weight);
        resultScaleOffset *= info.weight;
    }
}
