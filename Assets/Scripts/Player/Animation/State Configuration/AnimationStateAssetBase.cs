using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStateAnimationBase", menuName = "Player/Animation/PlayerStateAnimationBase")]
public class AnimationStateAssetBase : ScriptableObject
{
    [SerializeField] private AnimationCurve blendIn = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private float blendInDuration;
    [SerializeField] private AnimationCurve blendOut = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private float blendOutDuration;

    [SerializeField] private float speed = 1.0f;

    [SerializeField] private AnimationClip clipToPlay;
    [SerializeField] private ProceduralTransformAnimationAsset cameraAnimationAsset;
    [Tooltip("Для отключения анимации FOV установить его меньше нуля")]
    [SerializeField] private float targetFov = -1;
    [SerializeField] private ProceduralTransformAnimationAsset armsProceduralAnimationAsset;
    [Tooltip("Задает позу IK для анимации. Задать -1 для игнорирования позы IK")]
    [SerializeField] private int ikRigIndex = -1;

    [SerializeField] private bool isInfinite = false;
    [Tooltip("На время анимации влияет параметр Speed")]
    [SerializeField] private float animationTime = 1;
    [Tooltip("Имя следующей анимации в PlayerAnimationSystem")]
    [SerializeField] private string nextAnimationName;

    public AnimationCurve BlendIn => blendIn;
    public float BlendInDuration => blendInDuration;
    public AnimationCurve BlendOut => blendOut;
    public float BlendOutDuration => blendOutDuration;
    public float Speed => speed;
    public AnimationClip Clip => clipToPlay;
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
