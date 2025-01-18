using UnityEngine;
using UnityEngine.Playables;
using Unity.Cinemachine;

// A behaviour that is attached to a playable
public class CinemachinePropertyPlayableBehaviour : PlayableBehaviour
{
    private CinemachineCamera cameraInternal;
    private float defaultFOV;
    private float defaultDutch;

    public float targetFOV;
    public float targetDutch;

    private float TargetFOVOffset => targetFOV - defaultFOV;
    private float TargetDutchOffset => targetDutch - defaultDutch;

    public CinemachineCamera Camera
    {
        get
        {
            return cameraInternal;
        }
        set
        {
            cameraInternal = value;
            defaultFOV = cameraInternal.Lens.FieldOfView;
            defaultDutch = cameraInternal.Lens.Dutch;
        }
    }

    public override void PrepareFrame(Playable playable, FrameData info)
    {
        cameraInternal.Lens.Dutch = defaultDutch;
        cameraInternal.Lens.FieldOfView = defaultFOV;
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        cameraInternal.Lens.Dutch += TargetDutchOffset * info.weight;
        cameraInternal.Lens.FieldOfView += TargetFOVOffset * info.weight;
    }
}
