using UnityEngine;
using UnityEngine.Playables;
using Unity.Cinemachine;
using NUnit.Framework;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine.Animations.Rigging;
using UnityEngine.Animations;


[Serializable]
public struct AnimationAssetDescriptor
{
    public string animationName;
    public AnimationStateAssetBase asset;
}

public class PlayerAnimationSystem : MonoBehaviour
{
    [SerializeField] private PlayerMovementStateMachine movementController;
    [SerializeField] private Animator handsAnimator;
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
    private AnimationMixerPlayable animationClipMixer;


    public PlayerMovementStateMachine MovementController => movementController;

    private void Awake()
    {
        
    }

    private void Start()
    {
        ConfigureAnimationGraph();
        currentMovementStateName = defaultAnimationName;
        if (animationControllers.ContainsKey(currentMovementStateName))
            StartCoroutine(animationControllers[currentMovementStateName].BlendIn());
    }

    private void Update()
    {
        
    }

    private void LateUpdate()
    {
        foreach (var animState in animationControllers)
        {
            animState.Value.UpdateState(this, Time.deltaTime);
        }
        //foreach (var animState in animationControllers)
        //{
        //    animState.Value.UpdateState(this, Time.deltaTime);
        //}
        //meshRigBuilder.SyncLayers();
        //meshRigBuilder.Evaluate(Time.deltaTime);
        //graph.Evaluate(Time.deltaTime);

    }

    private void OnDestroy()
    {
        if (graph.IsValid())
            graph.Destroy();

        if (meshRigBuilder.graph.IsValid())
            meshRigBuilder.graph.Destroy();
    }

    private void ConfigureAnimationGraph()
    {

        graph = PlayableGraph.Create("Player Animation System");
        //graph.SetTimeUpdateMode(DirectorUpdateMode.Manual);

        int cameraPoseCount = 1 + animationStates.Where(descriptor => descriptor.asset.CameraAnimation != null).Count();
        int cameraPropertiesCount = 1 + animationStates.Where(descriptor => descriptor.asset.CameraFOV >= 0
                                                            || descriptor.asset.CameraAnimation != null).Count();
        int meshPoseCount = 1 + animationStates.Where(descriptor => descriptor.asset.ArmsProceduralAnimation != null).Count();
        int ikRigCount = 1 + animationStates.Where(descriptor => descriptor.asset.IKRigIndex >= 0).Count();

        int clipsCount = animationStates.Where(descriptor => descriptor.asset.Clip != null).Count();

        ScriptPlayableOutput generalCameraAnimationOutput = ScriptPlayableOutput.Create(graph, "General Camera Animation Output");
        generalCameraAnimationOutput.SetUserData(this);
        ScriptPlayableOutput generalHandsProceduralAnimationOutput = ScriptPlayableOutput.Create(graph, "General Hands ProceduralAnimation Output");
        generalHandsProceduralAnimationOutput.SetUserData(this);
        AnimationPlayableOutput animationPlayableOutput = AnimationPlayableOutput.Create(graph, "General Animation clip Output", handsAnimator);
        animationPlayableOutput.SetUserData(this);

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

        if (clipsCount > 0)
        {
            var meshLayerMixer = AnimationLayerMixerPlayable.Create(graph, 1);
            animationPlayableOutput.SetSourcePlayable(meshLayerMixer);
            animationClipMixer = AnimationMixerPlayable.Create(graph, clipsCount);
            meshLayerMixer.ConnectInput(0, animationClipMixer, 0);
            meshLayerMixer.SetInputWeight(0, 1);
        }

        var animatorOutput = AnimationPlayableOutput.Create(graph, "Animator for IK", handsAnimator);

        var animatorPlayable = AnimatorControllerPlayable.Create(graph, handsAnimator.runtimeAnimatorController);
        animatorOutput.SetSourcePlayable(animatorPlayable);
        animatorOutput.SetWeight(1);


        int cameraPosIndex = 0;
        int cameraPropIndex = 0;
        int meshPosIndex = 0;
        int ikRigIndex = 0;
        int clipIndex = 0;
        foreach (var animAsset in animationStates)
        {
            InitAnimationStateController(animAsset, ref cameraPosIndex, ref cameraPropIndex, 
                ref meshPosIndex, ref ikRigIndex, ref clipIndex);
        }

        graph.Play();
        //PlayableGraph IKgraph = PlayableGraph.Create("IK Graph");
        //IKgraph.SetTimeUpdateMode(DirectorUpdateMode.Manual);
        //meshRigBuilder.Build(IKgraph);

        //meshRigBuilder.graph.SetTimeUpdateMode(DirectorUpdateMode.Manual);
    }

    private void InitAnimationStateController(AnimationAssetDescriptor asset, 
        ref int cameraAnimationIndex, ref int cameraPropertyIndex, ref int handsPoseIndex, ref int ikRigIndex, ref int clipIndex)
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

        if (asset.asset.Clip != null)
        {
            AnimationClipPlayable clipPlayable = AnimationClipPlayable.Create(graph, asset.asset.Clip);
            clipPlayable.SetApplyFootIK(false);
            clipPlayable.SetApplyPlayableIK(false);
            int inputIndex = clipIndex;
            animationClipMixer.ConnectInput(inputIndex, clipPlayable, 0);
            animationClipMixer.SetInputWeight(inputIndex, 0);
            //if (asset.asset.IKRigIndex >= 0)
            //{
            //    clipPlayable.SetApplyPlayableIK(false);
            //}
            state.SetControll(animationClipMixer, inputIndex);
            clipIndex++;

        }

        animationControllers.Add(asset.animationName, state);
    }

    public void SetState(string stateName)
    {
        if (stateName != currentMovementStateName)
        {
            float blendTime = Mathf.Min(animationControllers[currentMovementStateName].Asset.BlendOutDuration,
                animationControllers[stateName].Asset.BlendInDuration);
            if (animationControllers.ContainsKey(currentMovementStateName))
                StartCoroutine(animationControllers[currentMovementStateName].BlendOut(blendTime));

            currentMovementStateName = stateName;

            if (animationControllers.ContainsKey(currentMovementStateName))
                StartCoroutine(animationControllers[currentMovementStateName].BlendIn(blendTime));
        }
    }
}
