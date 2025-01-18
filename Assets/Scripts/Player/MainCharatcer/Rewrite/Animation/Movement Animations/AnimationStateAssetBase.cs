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
    [Tooltip("Задает позу IK для анимации. Задать -1 для игнорирования позы IK")]
    [SerializeField] private int ikRigIndex = -1;

    [SerializeField] private bool isInfinite = false;
    [SerializeField] private float animationTime = 1;
    [Tooltip("Имя следующей анимации в AnimationSystem")]
    [SerializeField] private string nextAnimationName;

    public AnimationCurve BlendIn => blendIn;
    public float BlendInDuration => blendInDuration;
    public AnimationCurve BlendOut => blendOut;
    public float BlendOutDuration => blendOutDuration;
    public ProceduralTransformAnimationAsset CameraAnimation => cameraAnimationAsset;
    public float CameraFOV => targetFov;
    public ProceduralTransformAnimationAsset ArmsProceduralAnimation => armsProceduralAnimationAsset;

    public int IKRigIndex => ikRigIndex;

    public bool IsInfinite => isInfinite; 
    public float AnimationTime => animationTime; 
    public string NextAnimationName => nextAnimationName;

    public virtual AnimationStateControllerBase CreateState(PlayerAnimationSystem animSystem)
    {
        return new AnimationStateControllerBase(this);
    }
}
