using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHitVignette : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private Image vignette;
    [SerializeField] private AnimationCurve fadeOutOpacity;

    private float fadeoutTime;

    private float maxOutTime => fadeOutOpacity.keys[fadeOutOpacity.length - 1].time;

    private void OnEnable()
    {
        health.TookDamage.AddListener(OnTakeDamage);
        health.TookHeavyDamage.AddListener(OnTakeDamage);
    }

    private void OnDisable()
    {
        health.TookDamage.RemoveListener(OnTakeDamage);
        health.TookHeavyDamage.RemoveListener(OnTakeDamage);
    }

    private void Start()
    {
        fadeoutTime = maxOutTime;
    }

    private void Update()
    {
        if (fadeoutTime < maxOutTime)
        {
            fadeoutTime += Time.deltaTime;

            Color currentColor = vignette.color;
            currentColor.a = fadeOutOpacity.Evaluate(fadeoutTime);
            vignette.color = currentColor;
        }
    }

    private void OnTakeDamage()
    {
        fadeoutTime = 0;
    }
}
