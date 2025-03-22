using UnityEngine;
using UnityEngine.UI;

public class ButtonActiveUI : MonoBehaviour
{
    private GameObject activeUIObject;
    private GameObject deactiveUIObject;

    public void ActiveUIObject(GameObject activeUIObject)
    {
        this.activeUIObject = activeUIObject;
        activeUIObject.SetActive(true);
    }
    public void DeactivateUIObject(GameObject deactiveUIObject)
    {
        this.deactiveUIObject = deactiveUIObject;
        deactiveUIObject.SetActive(false);
    }
}
