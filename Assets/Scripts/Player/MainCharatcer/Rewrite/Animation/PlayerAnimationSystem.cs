using UnityEngine;
using UnityEngine.Playables;
using UnityEditor.Experimental.GraphView;
using Unity.Cinemachine;

public class PlayerAnimationSystem : MonoBehaviour
{
    [SerializeField] private PlayerMovementStateMachine movementController;
    [SerializeField] private CinemachineCamera playerCamera;
    [SerializeField] private Transform meshRoot;

    [SerializeField] private ProceduralTransformAnimationAsset baseCameraAnimation;
    [SerializeField] private ProceduralTransformAnimationAsset crouchCameraAnimation;

    [SerializeField] private float slideDutch;
    [SerializeField] private float slideFOV;

    private PlayableGraph graph;
    private ScriptPlayable<ProceduralTransformApplier> cameraRootPlayable;

    private Playable cameraPosMixer;
    private Playable cameraPropertyMixer;

    private void Awake()
    {
        ConfigureAnimationGraph();
    }

    private void Start()
    {

    }

    private void LateUpdate()
    {
        PlayerMovementStateType movementType = movementController.GetCurrentStateType();

        if (movementType == PlayerMovementStateType.Crouch || movementType == PlayerMovementStateType.Slide)
        {
            cameraPosMixer.SetInputWeight(0, 0);
            cameraPosMixer.SetInputWeight(1, 1);
        }
        else
        {
            cameraPosMixer.SetInputWeight(1, 0);
            cameraPosMixer.SetInputWeight(0, 1);
        }

        if (movementType == PlayerMovementStateType.Slide)
        {
            cameraPropertyMixer.SetInputWeight(0, 0);
            cameraPropertyMixer.SetInputWeight(1, 1);
        }
        else
        {
            cameraPropertyMixer.SetInputWeight(0, 1);
            cameraPropertyMixer.SetInputWeight(1, 0);
        }

        graph.Evaluate(Time.deltaTime);
    }

    private void OnDestroy()
    {
        if (graph.IsValid())
            graph.Destroy();
    }

    private void ConfigureAnimationGraph()
    {
        graph = PlayableGraph.Create("Player Animation System");
        graph.SetTimeUpdateMode(DirectorUpdateMode.Manual);

        ScriptPlayableOutput generalProceduralAnimationOutput = ScriptPlayableOutput.Create(graph, "General Procedural Animation Output");
        
        cameraRootPlayable = ScriptPlayable<ProceduralTransformApplier>.Create(graph, 2);
        generalProceduralAnimationOutput.SetSourcePlayable(cameraRootPlayable);

        cameraRootPlayable.GetBehaviour().handledTransform = playerCamera.Follow;

        cameraPosMixer = ScriptPlayable<ProceduralTransformMixer>.Create(graph, 2);
        cameraRootPlayable.ConnectInput(0, cameraPosMixer, 0);
        cameraRootPlayable.SetInputWeight(0, 1);

        var cameraBasePose = baseCameraAnimation.CreatePlayable(graph, gameObject);
        cameraPosMixer.ConnectInput(0, cameraBasePose, 0);
        cameraPosMixer.SetInputWeight(0, 1);
        var cameraCrouchPose = crouchCameraAnimation.CreatePlayable(graph, gameObject);
        cameraPosMixer.ConnectInput(1, cameraCrouchPose, 0);
        cameraPosMixer.SetInputWeight(1, 0);

        cameraPropertyMixer = Playable.Create(graph, 2);
        cameraRootPlayable.ConnectInput(1, cameraPropertyMixer, 0);
        cameraRootPlayable.SetInputWeight(1, 1);

        var cameraBaseProperty = ScriptPlayable<CinemachinePropertyPlayableBehaviour>.Create(graph);
        cameraBaseProperty.GetBehaviour().Camera = playerCamera;
        cameraBaseProperty.GetBehaviour().targetDutch = 0;
        cameraBaseProperty.GetBehaviour().targetFOV = playerCamera.Lens.FieldOfView;
        cameraPropertyMixer.ConnectInput(0, cameraBaseProperty, 0);
        cameraPropertyMixer.SetInputWeight(0, 1);

        var cameraSlideProperty = ScriptPlayable<CinemachinePropertyPlayableBehaviour>.Create(graph);
        cameraSlideProperty.GetBehaviour().Camera = playerCamera;
        cameraSlideProperty.GetBehaviour().targetDutch = slideDutch;
        cameraSlideProperty.GetBehaviour().targetFOV = slideFOV;
        cameraPropertyMixer.ConnectInput(1, cameraSlideProperty, 0);
        cameraPropertyMixer.SetInputWeight(1, 0);

        graph.Play();
    }
}
