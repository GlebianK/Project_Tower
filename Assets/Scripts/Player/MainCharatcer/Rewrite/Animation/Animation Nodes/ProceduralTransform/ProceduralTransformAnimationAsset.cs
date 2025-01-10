using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public struct ProceduralValueAnimationProperties
{
    public AnimationCurve offsetCurve;
    public float multiplier;
}

[System.Serializable]
public struct ProceduralVector3AnimationProperties
{
    public ProceduralValueAnimationProperties x;
    public ProceduralValueAnimationProperties y;
    public ProceduralValueAnimationProperties z;
}

[System.Serializable]
[CreateAssetMenu(fileName = "NewProceduralAnimation", menuName = "Player/Animation/Procedural Transform Animation")]
public class ProceduralTransformAnimationAsset : PlayableAsset
{
    [Header("Local Target Offsets")]
    [SerializeField] private Vector3 targetOffset;
    [SerializeField] private Quaternion targetRotationOffset = Quaternion.identity;
    [SerializeField] private Vector3 targetLocalScaleOffset;

    [Header("Runtime Procedural Offsets")]
    [SerializeField] private ProceduralVector3AnimationProperties proceduralPosition;
    [SerializeField] private ProceduralVector3AnimationProperties proceduralRotation;
    [SerializeField] private ProceduralVector3AnimationProperties proceduralScale;

    public Vector3 TargetOffset => targetOffset;
    public Quaternion TargetRotationOffset => targetRotationOffset;
    public Vector3 TargetLocalScaleOffset => targetLocalScaleOffset;
    public ProceduralVector3AnimationProperties ProceduralPosition => proceduralPosition;
    public ProceduralVector3AnimationProperties ProceduralRotation => proceduralRotation;
    public ProceduralVector3AnimationProperties ProceduralScale => proceduralScale;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
    {
        var playable = ScriptPlayable<ProceduralTransformAnimationBehaviour>.Create(graph);
        
        ProceduralTransformAnimationBehaviour behaviour = playable.GetBehaviour();
        behaviour.targetLocalPositionOffset = targetOffset;
        behaviour.targetLocalRotationOffset = targetRotationOffset;
        behaviour.targetLocalScaleOffset = targetLocalScaleOffset;

        behaviour.proceduralPosition = proceduralPosition;
        behaviour.proceduralRotation = proceduralRotation;
        behaviour.proceduralScale = proceduralScale;
        //configure behaviour here

        return playable;
    }

    
}
