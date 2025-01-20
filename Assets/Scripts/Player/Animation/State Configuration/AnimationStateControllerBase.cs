using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Playables;

public class AnimationStateControllerBase
{
    private AnimationStateAssetBase asset;

    private List<Tuple<Playable, int>> controlledWeights;

    private float currentWeight = 0;
    private float internalTimer = 0;

    private bool isActive = false;

    public AnimationStateControllerBase(AnimationStateAssetBase asset)
    {
        this.asset = asset;
        controlledWeights = new List<Tuple<Playable, int>>();
    }

    public virtual void SetControll(Playable playable, int inputPort)
    {
        controlledWeights.Add(new Tuple<Playable, int>(playable, inputPort));
    }

    public virtual void UpdateState(PlayerAnimationSystem system, float deltaTime)
    {
        if (!asset.IsInfinite && isActive)
        {
            internalTimer += deltaTime;
            if (internalTimer > asset.AnimationTime - asset.BlendOutDuration && isActive)
            {
                system.SetState(asset.NextAnimationName);
            }
        } 
        else
        {
            internalTimer = 0;
        }
    }

    public IEnumerator BlendIn()
    {
        internalTimer = 0;
        isActive = true;
        yield return Blend(asset.BlendIn, asset.BlendInDuration);
    }

    public IEnumerator BlendOut()
    {
        isActive = false;
        yield return Blend(asset.BlendOut, asset.BlendOutDuration);
        internalTimer = 0;
    }

    private IEnumerator Blend(AnimationCurve curve, float duration)
    {
        if (duration <= 0)
        {
            SetAllWeights(curve.Evaluate(1));
        }
        else
        {
            float prevTime = 0;
            float prevWeight = curve.Evaluate(prevTime / duration);
            while (prevTime <= duration)
            {
                float time = prevTime + Time.deltaTime;
                float weight = curve.Evaluate(time / duration);
                AddAllWeights(weight - prevWeight);
                prevTime = time;
                prevWeight = weight;
                yield return null;
            }
            float finalWeight = curve.Evaluate(1);
            AddAllWeights(finalWeight - prevWeight);
        }
    }

    private void AddAllWeights(float weight)
    {
        float prevWeight = currentWeight;
        currentWeight = prevWeight + weight;

        foreach (var playableTuple in  controlledWeights)
        {
            var playable = playableTuple.Item1;
            var inputIndex = playableTuple.Item2;

            playable.SetInputWeight(inputIndex, Mathf.Clamp01(currentWeight));
        }

    }

    private void SetAllWeights(float weight)
    {
        foreach (var playableTuple in controlledWeights)
        {
            var playable = playableTuple.Item1;
            var inputIndex = playableTuple.Item2;

            currentWeight = weight;
            playable.SetInputWeight(inputIndex, Mathf.Clamp01(currentWeight));
        }
    }
}
