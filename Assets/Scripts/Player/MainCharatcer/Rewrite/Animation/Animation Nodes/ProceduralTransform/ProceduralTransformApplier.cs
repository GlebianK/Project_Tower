using UnityEngine;
using UnityEngine.Playables;

// A behaviour that is attached to a playable
public class ProceduralTransformApplier : PlayableBehaviour
{
    public Transform handledTransform;

    private Vector3 oldOffset = Vector3.zero;
    private Quaternion oldRot = Quaternion.identity;
    private Vector3 oldScale = Vector3.zero;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        ProceduralTransformAnimationBehaviourBase input = ((ScriptPlayable<ProceduralTransformAnimationBehaviourBase>)playable.GetInput(0)).GetBehaviour();

        Vector3 newPosition = input.position * info.weight;
        Quaternion newRotation = Quaternion.Slerp(Quaternion.identity, input.rotation, info.weight);
        Vector3 newScale = input.scale * info.weight;

        Quaternion rotOffset = newRotation * Quaternion.Inverse(oldRot);

        handledTransform.localPosition += newPosition - oldOffset;
        handledTransform.localRotation = rotOffset * handledTransform.localRotation;
        handledTransform.localScale += newScale;

        oldOffset = newPosition;
        oldRot = newRotation;
        oldScale = newScale;
    }
}
