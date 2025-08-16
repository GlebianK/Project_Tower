using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

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

    [Header("Time")]
    [SerializeField] private bool enableTimeoutBadEnd;

    [Header("Enemies")]
    [SerializeField] private int enemyKillsForGoodEnd;
    [InspectorName("Count Rat")]
    [SerializeField] private bool ratKillCounted;
    [InspectorName("Count Light Enemy")]
    [SerializeField] private bool lightKillCounted;
    [InspectorName("Count Heavy Enemy")]
    [SerializeField] private bool heavyKillCounted;

    [Header("Health Kits")]
    [SerializeField] private int healthKitsForGoodEnd;

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
        GameManager.Instance.StopGameplay();

        LostGameEvent.Invoke();
    }

    public void OnPlayerReachedEnd()
    {
        GameManager.Instance.StopGameplay();

        ShowEnding();
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

    public void ShowStatsOnComplete()
    {
        CompleteGameEvent?.Invoke();
    }

    private void ShowEnding()
    {
        if (enableTimeoutBadEnd && timer.Time <= 0)
        {
            OnGameBadEnded?.Invoke();
            return;
        }

        int totalKills = GetEnemyKillsCounted();
        if (totalKills < enemyKillsForGoodEnd)
        {
            OnGameBadEnded?.Invoke();
            return;
        }

        if (HealthkitCollected < healthKitsForGoodEnd)
        {
            OnGameNormEnded?.Invoke();
            return;
        }

        OnGameGoodEnded?.Invoke();
    }

    public int GetEnemyKillsCounted()
    {
        int totalKills = 0;
        if (ratKillCounted)
        {
            totalKills += RatKilled;
        }
        if (lightKillCounted)
        {
            totalKills += LightKilled;
        }
        if (heavyKillCounted)
        {
            totalKills += HeavyKilled;
        }

        return totalKills;
    }

    // =============================== ÊÎÑÒÛËÜ ÄËß ×ÅÊÏÎÈÍÒÀ
    public int GetRatKillsCounted()
    {
        if (ratKillCounted)
            return RatKilled;
        else
            return 0;
    }

    public void SetRatKillsCounted(int amount)
    {
        RatKilled = amount;
    }

    public int GetLightEnemiesKillsCounted()
    {
        if (lightKillCounted)
            return LightKilled;
        else
            return 0;
    }

    public void SetLightEnemiesKillsCounted(int amount)
    {
        LightKilled = amount;
    }

    public int GetShieldEnemiesKillsCounted()
    {
        if (heavyKillCounted)
            return HeavyKilled;
        else
            return 0;
    }

    public void SetShieldEnemiesKillsCounted(int amount)
    {
        HeavyKilled = amount;
    }
}
