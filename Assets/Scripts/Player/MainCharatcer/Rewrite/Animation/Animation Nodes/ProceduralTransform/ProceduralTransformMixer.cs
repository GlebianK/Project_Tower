using UnityEngine;
using UnityEngine.Playables;

public class ProceduralTransformMixer : ProceduralTransformAnimationBehaviourBase
{
    private Vector3 resultOffset = Vector3.zero;
    private Quaternion resultRotation = Quaternion.identity;
    private Vector3 resultScale = Vector3.zero;

    public override Vector3 position => resultOffset;

    public override Quaternion rotation => resultRotation;

    public override Vector3 scale => resultScale;

    public override void PrepareFrame(Playable playable, FrameData info)
    {
        resultOffset = Vector3.zero;
        resultRotation = Quaternion.identity;
        resultScale = Vector3.zero;

        int childCount = playable.GetInputCount();

        for (int i = 0; i < childCount; i++)
        {
            ProceduralTransformAnimationBehaviourBase input;
            input = ((ScriptPlayable<ProceduralTransformAnimationBehaviour>)playable.GetInput(i)).GetBehaviour();
            if (input == null)
            {
                input = ((ScriptPlayable<ProceduralTransformMixer>)playable.GetInput(i)).GetBehaviour();
                if (input == null)
                    return;
                input = ((ScriptPlayable<ProceduralTransformMixer>)playable.GetInput(i)).GetBehaviour();
            }

            resultOffset += input.position * info.weight;
            resultRotation = Quaternion.Slerp(Quaternion.identity, input.rotation, info.weight) * resultRotation;
            resultScale += input.scale * info.weight;
        }
    }
}
