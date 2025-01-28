using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;
using UnityEngine.Playables;

public class AnimationStateControllerBase
{
    private AnimationStateAssetBase asset;

    private List<Tuple<Playable, int>> controlledWeights;
    private Playable clip;

    private float currentWeight = 0;
    private float internalTimer = 0;

    private bool isActive = false;

    public AnimationStateAssetBase Asset => asset;

    public AnimationStateControllerBase(AnimationStateAssetBase asset)
    {
        this.asset = asset;
        controlledWeights = new List<Tuple<Playable, int>>();
    }

    public virtual void SetControll(Playable playable, int inputPort)
    {
        controlledWeights.Add(new Tuple<Playable, int>(playable, inputPort));

        playable.GetInput(inputPort).Pause();
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

    public IEnumerator BlendIn(float blendTime = 0)
    {
        internalTimer = 0;
        isActive = true;
        foreach (var controlledWeight in controlledWeights)
        {
            Playable mixer = controlledWeight.Item1;
            int inputIndex = controlledWeight.Item2;
            Playable playable = mixer.GetInput(inputIndex);

            playable.Play();
            playable.SetTime(0);
        }
        yield return Blend(asset.BlendIn, blendTime);
    }

    public IEnumerator BlendOut(float blendTime = 0)
    {
        isActive = false;
        yield return Blend(asset.BlendOut, blendTime);
        internalTimer = 0;
        foreach (var controlledWeight in controlledWeights)
        {
            Playable mixer = controlledWeight.Item1;
            int inputIndex = controlledWeight.Item2;
            mixer.GetInput(inputIndex).Pause();
        }
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
