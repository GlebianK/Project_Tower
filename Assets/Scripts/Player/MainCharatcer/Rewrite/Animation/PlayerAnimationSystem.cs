using UnityEngine;
using UnityEngine.Playables;
using Unity.Cinemachine;
using NUnit.Framework;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine.Animations.Rigging;

[Serializable]
public struct AnimationAssetDescriptor
{
    public string animationName;
    public AnimationStateAssetBase asset;
}

public class PlayerAnimationSystem : MonoBehaviour
{
    [SerializeField] private PlayerMovementStateMachine movementController;
    [SerializeField] private CinemachineCamera playerCamera;
    [SerializeField] private Transform meshRoot;
    [SerializeField] private RigBuilder meshRigBuilder;

    [SerializeField] private List<AnimationAssetDescriptor> animationStates;
    [SerializeField] private string defaultAnimationName;

    private Dictionary<string, AnimationStateControllerBase> animationControllers
        = new Dictionary<string, AnimationStateControllerBase>();
    private string currentMovementStateName;
    private PlayableGraph graph;

    private Playable cameraPosMixer;
    private Playable cameraPropertyMixer;
    private Playable handsPosMixer;
    private Playable meshIKMixer;

    public PlayerMovementStateMachine MovementController => movementController;

    private void Awake()
    {
        ConfigureAnimationGraph();
    }

    private void Start()
    {
        currentMovementStateName = defaultAnimationName;
        if (animationControllers.ContainsKey(currentMovementStateName))
            StartCoroutine(animationControllers[currentMovementStateName].BlendIn());
    }

    private void Update()
    {
        foreach (var animState in animationControllers)
        {
            animState.Value.UpdateState(this, Time.deltaTime);
        }
    }

    private void LateUpdate()
    {
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

        int cameraPoseCount = 1 + animationStates.Where(descriptor => descriptor.asset.CameraAnimation != null).Count();
        int cameraPropertiesCount = 1 + animationStates.Where(descriptor => descriptor.asset.CameraFOV >= 0
                                                            || descriptor.asset.CameraAnimation != null).Count();
        int meshPoseCount = 1 + animationStates.Where(descriptor => descriptor.asset.ArmsProceduralAnimation != null).Count();
        int ikRigCount = 1 + animationStates.Where(descriptor => descriptor.asset.IKRigIndex >= 0).Count();


        ScriptPlayableOutput generalCameraAnimationOutput = ScriptPlayableOutput.Create(graph, "General Camera Animation Output");
        generalCameraAnimationOutput.SetUserData(this);
        ScriptPlayableOutput generalHandsProceduralAnimationOutput = ScriptPlayableOutput.Create(graph, "General Hands ProceduralAnimation Output");
        generalHandsProceduralAnimationOutput.SetUserData(this);
        // setup camera graph mix nodes
        var cameraRootPlayable = ScriptPlayable<ProceduralTransformApplier>.Create(graph, 2);
        generalCameraAnimationOutput.SetSourcePlayable(cameraRootPlayable);

        cameraRootPlayable.GetBehaviour().handledTransform = playerCamera.Follow;

        cameraPosMixer = ScriptPlayable<ProceduralTransformMixer>.Create(graph, cameraPoseCount);
        cameraRootPlayable.ConnectInput(0, cameraPosMixer, 0);
        cameraRootPlayable.SetInputWeight(0, 1);
        cameraPropertyMixer = Playable.Create(graph, cameraPropertiesCount);
        cameraRootPlayable.ConnectInput(1, cameraPropertyMixer, 0);
        cameraRootPlayable.SetInputWeight(1, 1);

        // setup mesh graph mix nodes
        var handsRootPlayable = ScriptPlayable<ProceduralTransformApplier>.Create(graph, 2);
        generalHandsProceduralAnimationOutput.SetSourcePlayable(handsRootPlayable);

        handsRootPlayable.GetBehaviour().handledTransform = meshRoot;

        handsPosMixer = ScriptPlayable<ProceduralTransformMixer>.Create(graph, meshPoseCount);
        handsRootPlayable.ConnectInput(0, handsPosMixer, 0);
        handsRootPlayable.SetInputWeight(0, 1);

        meshIKMixer = Playable.Create(graph, ikRigCount);
        handsRootPlayable.ConnectInput(1, meshIKMixer, 0);
        handsRootPlayable.SetInputWeight(1, 1);

        int cameraPosIndex = 0;
        int cameraPropIndex = 0;
        int meshPosIndex = 0;
        int ikRigIndex = 0;
        foreach (var animAsset in animationStates)
        {
            InitAnimationStateController(animAsset, ref cameraPosIndex, ref cameraPropIndex, 
                ref meshPosIndex, ref ikRigIndex);
        }

        graph.Play();
        //meshIKgraph.Play();
    }

    private void InitAnimationStateController(AnimationAssetDescriptor asset, 
        ref int cameraAnimationIndex, ref int cameraPropertyIndex, ref int handsPoseIndex, ref int ikRigIndex)
    {
        AnimationStateControllerBase state = asset.asset.CreateState(this);

        if (asset.asset.CameraFOV >= 0 || asset.asset.CameraAnimation != null)
        {
            var cameraProperty = ScriptPlayable<CinemachinePropertyPlayableBehaviour>.Create(graph);
            cameraProperty.GetBehaviour().Camera = playerCamera;
            cameraProperty.GetBehaviour().targetDutch = asset.asset.CameraAnimation != null
                ? asset.asset.CameraAnimation.TargetRotationOffset.eulerAngles.z : 0;
            cameraProperty.GetBehaviour().targetFOV = asset.asset.CameraFOV >= 0
                ? asset.asset.CameraFOV : playerCamera.Lens.FieldOfView;

            int inputIndex = cameraPropertyIndex;
            cameraPropertyMixer.ConnectInput(inputIndex, cameraProperty, 0);
            state.SetControll(cameraPropertyMixer, inputIndex);
            cameraPropertyIndex++;
        }

        if (asset.asset.CameraAnimation != null)
        {
            var cameraPlayable = asset.asset.CameraAnimation.CreatePlayable(graph, gameObject);
            // flush z rotation because of dutch camera property
            var behaviour = ((ScriptPlayable<ProceduralTransformAnimationBehaviour>)cameraPlayable).GetBehaviour();
            Vector3 targetCameraRotationEuler = behaviour.targetLocalRotationOffset.eulerAngles;
            targetCameraRotationEuler.z = 0;
            behaviour.targetLocalRotationOffset = Quaternion.Euler(targetCameraRotationEuler);

            int inputIndex = cameraAnimationIndex;
            cameraPosMixer.ConnectInput(inputIndex, cameraPlayable, 0);
            state.SetControll(cameraPosMixer, inputIndex);
            cameraAnimationIndex++;
        }

        if (asset.asset.ArmsProceduralAnimation != null)
        {
            var handsPos = asset.asset.ArmsProceduralAnimation.CreatePlayable(graph, gameObject);

            int inputIndex = handsPoseIndex;
            handsPosMixer.ConnectInput(inputIndex, handsPos, 0);
            state.SetControll(handsPosMixer, inputIndex);
            handsPoseIndex++;
        }

        if (asset.asset.IKRigIndex >= 0)
        {
            var rigPlayable = ScriptPlayable<RigLayerPlayable>.Create(graph);

            RigLayerPlayable behaviour = rigPlayable.GetBehaviour();
            behaviour.controlledRig = meshRigBuilder.layers[asset.asset.IKRigIndex].rig;

            int inputIndex = ikRigIndex;
            meshIKMixer.ConnectInput(inputIndex, rigPlayable, 0);
            state.SetControll(meshIKMixer, inputIndex);
            ikRigIndex++;
        }

        animationControllers.Add(asset.animationName, state);
    }

    public void SetState(string stateName)
    {
        if (stateName != currentMovementStateName)
        {
            if (animationControllers.ContainsKey(currentMovementStateName))
                StartCoroutine(animationControllers[currentMovementStateName].BlendOut());

            currentMovementStateName = stateName;

            if (animationControllers.ContainsKey(currentMovementStateName))
                StartCoroutine(animationControllers[currentMovementStateName].BlendIn());
        }
    }
}
