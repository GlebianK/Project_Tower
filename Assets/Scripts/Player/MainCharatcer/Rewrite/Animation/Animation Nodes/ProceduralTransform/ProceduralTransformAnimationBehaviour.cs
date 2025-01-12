using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Playables;

public class ProceduralTransformAnimationBehaviourBase : PlayableBehaviour
{
    public virtual Vector3 position { get => Vector3.zero; }
    public virtual Quaternion rotation { get => Quaternion.identity; }
    public virtual Vector3 scale { get => Vector3.zero; }
}

// A behaviour that is attached to a playable
public class ProceduralTransformAnimationBehaviour : ProceduralTransformAnimationBehaviourBase
{
    private float time = 0;

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
        resultPositionOffset = Vector3.zero;
        resultRotationOffset = Quaternion.identity;
        resultScaleOffset = Vector3.zero;

        time += Time.deltaTime * info.effectiveSpeed;

        resultPositionOffset += GetPropertiesOffsetByTime(proceduralPosition, time);
        resultRotationOffset = Quaternion.Euler(GetPropertiesOffsetByTime(proceduralRotation, time)) * resultRotationOffset;
        resultScaleOffset += GetPropertiesOffsetByTime(proceduralScale, time);

        resultPositionOffset += targetLocalPositionOffset;
        resultRotationOffset = targetLocalRotationOffset * resultRotationOffset;
        resultScaleOffset += targetLocalScaleOffset;

        resultPositionOffset *= info.weight;
        resultRotationOffset = Quaternion.Slerp(Quaternion.identity, resultRotationOffset, info.weight);
        resultScaleOffset *= info.weight;
    }

    private Vector3 GetPropertiesOffsetByTime(in ProceduralVector3AnimationProperties properties, float time)
    {
        float x = GetOffsetByTime(properties.x, time);
        float y = GetOffsetByTime(properties.y, time);
        float z = GetOffsetByTime(properties.z, time);

        return new Vector3(x, y, z);
    }

    private float GetOffsetByTime(in ProceduralValueAnimationProperties properties, float time)
    {
        if (properties.offsetCurve.length == 0) 
            return 0;

        float maxTime = properties.offsetCurve.keys[properties.offsetCurve.length - 1].time;

        float loopedTime = time % maxTime;

        return properties.offsetCurve.Evaluate(loopedTime) * properties.multiplier;
    }
}
