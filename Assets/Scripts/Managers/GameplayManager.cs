using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public enum EnemyType
{
    Rat,
    Light,
    Heavy
}

public class GameplayManager : MonoBehaviour
{
    [SerializeField] private GameTimer timer;
    [Header("Finals")]
    [SerializeField] private int badEndMaximumScore;
    [SerializeField] private int goodEndMinimumScore;

    [Header("Score Rewards")]
    [InspectorName("For Rat")]
    [SerializeField] private int ratEnemyScoreReward;
    [InspectorName("For Enemy")]
    [SerializeField] private int lightEnemyScoreReward;
    [InspectorName("For Heavy")]
    [SerializeField] private int heavyEnemyScoreReward;
    [InspectorName("For HealthKit")]
    [SerializeField] private int healthkitScoreReward;

    [Header("Time reward")]
    [SerializeField] private int scoresPerRemainingSeconds;
    [SerializeField] private int maxScoresForTime;

    public float Score { get
        {
            return Mathf.Max(Mathf.Min((int)timer.Time * maxScoresForTime, maxScoresForTime), 0)
                + RatKilled * ratEnemyScoreReward
                + LightKilled * lightEnemyScoreReward
                + HeavyKilled * heavyEnemyScoreReward
                + HealthkitCollected * healthkitScoreReward;
        }
    }

    public int HealthkitCollected { get; private set; }
    public int RatKilled { get; private set; }
    public int LightKilled { get; private set; }
    public int HeavyKilled { get; private set; }

    public UnityEvent LostGameEvent;
    public UnityEvent CompleteGameEvent;

    public UnityEvent OnGameBadEnded;
    public UnityEvent OnGameNormEnded;
    public UnityEvent OnGameGoodEnded;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OnPlayerDied()
    {
        StopGameplay();

        LostGameEvent.Invoke();
    }

    public void OnPlayerReachedEnd()
    {
        StopGameplay();

        CompleteGameEvent.Invoke();
    }

    public void OnHealthKitPickup()
    {
        HealthkitCollected++;
    }

    public void OnHealthKitUsed()
    {
        HealthkitCollected--;
    }

    public void OnEnemyKilled(EnemyType type)
    {
        switch (type)
        {
            case EnemyType.Rat:
                RatKilled++;
                break;
            case EnemyType.Light:
                LightKilled++; 
                break;
            case EnemyType.Heavy:
                HeavyKilled++;
                break;
        }
    }

    public void ShowEnding()
    {
        if (Score < badEndMaximumScore)
        {
            OnGameBadEnded?.Invoke();
        }
        else
        {
            if (Score >= goodEndMinimumScore)
                OnGameGoodEnded?.Invoke();
            else
                OnGameNormEnded?.Invoke();
        }
    }
    private void StopGameplay()
    {
        Time.timeScale = 0;
        Time.fixedDeltaTime = 0;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
