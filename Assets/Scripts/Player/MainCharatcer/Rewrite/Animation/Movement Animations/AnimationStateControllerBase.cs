using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class AnimationStateControllerBase
{
    private AnimationStateAssetBase asset;

    private List<Tuple<Playable, int>> controlledWeights;

    public AnimationStateControllerBase(AnimationStateAssetBase asset)
    {
        this.asset = asset;
        controlledWeights = new List<Tuple<Playable, int>>();
    }

    public void SetControll(Playable playable, int inputPort)
    {
        controlledWeights.Add(new Tuple<Playable, int>(playable, inputPort));
    }

    public virtual void UpdateState(PlayerAnimationSystem system, float deltaTime)
    {

    }

    public IEnumerator BlendIn()
    {
        yield return Blend(asset.BlendIn, asset.BlendInDuration);
        SetAllWeights(1);
    }

    public IEnumerator BlendOut()
    {
        yield return Blend(asset.BlendOut, asset.BlendOutDuration);
        SetAllWeights(0);
    }

    private IEnumerator Blend(AnimationCurve curve, float duration)
    {
        float prevTime = 0;
        float prevWeight = curve.Evaluate(prevTime / duration);
        while (prevTime < duration)
        {
            float time = prevTime + Time.deltaTime;
            float weight = curve.Evaluate(time / duration);
            AddAllWeights(weight - prevWeight);
            prevTime = time;
            prevWeight = weight;
            yield return null;
        }
    }

    private void AddAllWeights(float weight)
    {
        foreach (var playableTuple in  controlledWeights)
        {
            var playable = playableTuple.Item1;
            var inputIndex = playableTuple.Item2;

            float prevWeight = playable.GetInputWeight(inputIndex);

            playable.SetInputWeight(inputIndex, prevWeight + weight);
        }
    }

    private void SetAllWeights(float weight)
    {
        foreach (var playableTuple in controlledWeights)
        {
            var playable = playableTuple.Item1;
            var inputIndex = playableTuple.Item2;

            playable.SetInputWeight(inputIndex, Mathf.Clamp01(weight));
        }
    }
}
