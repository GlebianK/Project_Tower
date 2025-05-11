using UnityEngine;

public class StatusView : MonoBehaviour
{
    [SerializeField] private GameObject toxicImage;
    [SerializeField] private GameObject radiationImage;

    private DangerousZone[] dangerousZones;

    // счётчики количества касающихся игрока зон для оставления UI включённым при переходе между одинковыми зонами
    private int inMudZonesCounter = 0;
    private int inLightZonesCounter = 0;

    private void Start()
    {
        toxicImage.SetActive(false);
        radiationImage.SetActive(false);
        inMudZonesCounter = 0;
        inLightZonesCounter = 0;

        dangerousZones = FindObjectsByType<DangerousZone>(FindObjectsSortMode.None);
        if (dangerousZones != null && dangerousZones.Length > 0)
        {
            foreach (DangerousZone dangerousZone in dangerousZones)
            {
                dangerousZone.ObjectEntered.AddListener(ActivateStatus);
                dangerousZone.ObjectExited.AddListener(DeactivateStatus);
            }
        }
    }

    private void OnDisable()
    {
        if (dangerousZones != null && dangerousZones.Length > 0)
        {
            foreach (DangerousZone dangerousZone in dangerousZones)
            {
                dangerousZone.ObjectEntered.RemoveListener(ActivateStatus);
                dangerousZone.ObjectExited.RemoveListener(DeactivateStatus);
            }
        }
    }

    // коллбэки для подключки к грязи и отключки от неё
    public void ActivateStatus(DangerousZone.DangerType type)
    {
        if (type == DangerousZone.DangerType.light)
        {
            inLightZonesCounter++;
            radiationImage.SetActive(true);
        }
        else if (type == DangerousZone.DangerType.mud)
        {
            inMudZonesCounter++;
            toxicImage.SetActive(true);
        }
        else
            Debug.LogError("StatusView -> ActivateStatus: something wrong with callbacks or danger zones!");
    }

    public void DeactivateStatus(DangerousZone.DangerType type)
    {        
        if (type == DangerousZone.DangerType.mud)
        {
            inMudZonesCounter--;
            if (inMudZonesCounter == 0)
                toxicImage.SetActive(false);
            else if (inMudZonesCounter < 0)
                Debug.LogError("Negative mud zones counter!");
        }
        else if (type == DangerousZone.DangerType.light)
        {
            inLightZonesCounter--;
            if (inLightZonesCounter == 0)
                radiationImage.SetActive(false);
            else if (inLightZonesCounter < 0)
                Debug.LogError("Negative light zones counter!");
        }
        else
            Debug.LogError("StatusView -> DeactivateStatus: something wrong with callbacks or danger zones!");
    }
}

