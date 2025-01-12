using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStateAnimationBase", menuName = "Player/Animation/PlayerStateAnimationBase")]
public class AnimationStateAssetBase : ScriptableObject
{
    [SerializeField] private AnimationCurve blendIn;
    [SerializeField] private float blendInDuration;
    [SerializeField] private AnimationCurve blendOut;
    [SerializeField] private float blendOutDuration;

    [SerializeField] private ProceduralTransformAnimationAsset cameraAnimationAsset;
    [Tooltip("Для отключения анимации FOV установить его меньше нуля")]
    [SerializeField] private float targetFov;
    [SerializeField] private ProceduralTransformAnimationAsset armsProceduralAnimationAsset;

    public AnimationCurve BlendIn => blendIn;
    public float BlendInDuration => blendInDuration;
    public AnimationCurve BlendOut => blendOut;
    public float BlendOutDuration => blendOutDuration;
    public ProceduralTransformAnimationAsset CameraAnimation => cameraAnimationAsset;
    public float CameraFOV => targetFov;
    public ProceduralTransformAnimationAsset ArmsProceduralAnimation => armsProceduralAnimationAsset;

    public virtual AnimationStateControllerBase CreateState(PlayerAnimationSystem animSystem)
    {
        return new AnimationStateControllerBase(this);
    }
}
