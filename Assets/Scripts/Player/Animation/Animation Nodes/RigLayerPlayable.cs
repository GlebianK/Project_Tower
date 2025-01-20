using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Playables;

// A behaviour that is attached to a playable
public class RigLayerPlayable : PlayableBehaviour
{
    public Rig controlledRig;
    // Called when the owning graph starts playing
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        controlledRig.weight = info.weight;
    }
}
