using UnityEngine;

public class StatusView : MonoBehaviour
{
    [SerializeField] private GameObject toxicImage;
    [SerializeField] private GameObject radiationImage;

    private DangerousZone[] dangerousZones;

    private void Start()
    {
        toxicImage.SetActive(false);
        radiationImage.SetActive(false);

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
            radiationImage.SetActive(true);
        else if (type == DangerousZone.DangerType.mud)
            toxicImage.SetActive(true);
        else
            Debug.LogError("StatusView -> ActivateStatus: something wrong with callbacks or danger zones!");
    }

    public void DeactivateStatus(DangerousZone.DangerType type)
    {
        if (type == DangerousZone.DangerType.mud)
            toxicImage.SetActive(false);
        else if (type == DangerousZone.DangerType.light)
            radiationImage.SetActive(false);
        else
            Debug.LogError("StatusView -> DeactivateStatus: something wrong with callbacks or danger zones!");
    }
}

