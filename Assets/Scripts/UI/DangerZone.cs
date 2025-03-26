using UnityEngine;
using UnityEngine.UI;

public class DangerZone : MonoBehaviour
{
    [SerializeField] private Image dangerZoneIcon;

    public void ActiveIconDangerZone()
    {
        dangerZoneIcon.enabled = true;
    } 
    public void DeactiveIconDangerZone()
    {
        dangerZoneIcon.enabled = false;
    }
}
