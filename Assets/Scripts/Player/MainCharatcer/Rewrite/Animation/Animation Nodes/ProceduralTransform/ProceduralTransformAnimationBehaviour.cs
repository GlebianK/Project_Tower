using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Playables;

public class ProceduralTransformAnimationBehaviourBase : PlayableBehaviour
{
    public virtual Vector3 position { get => Vector3.zero; }
    public virtual Quaternion rotation { get => Quaternion.identity; }
    public virtual Vector3 scale { get => Vector3.zero; }
    public float speed = 1;
}

// A behaviour that is attached to a playable
public class ProceduralTransformAnimationBehaviour : ProceduralTransformAnimationBehaviourBase
{
    public Vector3 targetLocalPositionOffset;
    public Quaternion targetLocalRotationOffset = Quaternion.identity;
    public Vector3 targetLocalScaleOffset;

    public ProceduralVector3AnimationProperties proceduralPosition;
    public ProceduralVector3AnimationProperties proceduralRotation;
    public ProceduralVector3AnimationProperties proceduralScale;

    private Vector3 resultPositionOffset = Vector3.zero;
    private Quaternion resultRotationOffset = Quaternion.identity;
    private Vector3 resultScaleOffset = Vector3.zero;

    public override Vector3 position => resultPositionOffset;

    public override Quaternion rotation => resultRotationOffset;

    public override Vector3 scale => resultScaleOffset;

    public override void PrepareFrame(Playable playable, FrameData info)
    {
        resultPositionOffset = targetLocalPositionOffset * info.weight;
        resultRotationOffset = Quaternion.Slerp(Quaternion.identity, targetLocalRotationOffset, info.weight);
        resultScaleOffset = targetLocalScaleOffset * info.weight;
    }
}
