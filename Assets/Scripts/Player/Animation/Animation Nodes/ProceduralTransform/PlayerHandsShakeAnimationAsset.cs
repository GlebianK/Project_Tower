using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
[CreateAssetMenu(fileName = "NewWalkProceduralAnimation", menuName = "Player/Animation/Walk Procedural")]
public class PlayerHandsShakeAnimationAsset : ProceduralTransformAnimationAsset
{
    [SerializeField] private Vector3 velocityDirectionMultiplier;

    // Factory method that generates a playable based on this asset
    public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
    {
        var playable = ScriptPlayable<PlayerHandsShakeAnimationBehaviour>.Create(graph);
        PlayerHandsShakeAnimationBehaviour behaviour = playable.GetBehaviour();
        behaviour.directionMultiplier = velocityDirectionMultiplier;
        behaviour.movementController = go.GetComponent<PlayerMovementStateMachine>();
        //configure behaviour here
        SetPropertiesTo(behaviour);

        return playable;
    }
}
