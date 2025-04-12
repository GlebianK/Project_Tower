using UnityEngine;

public class BackButton : MonoBehaviour
{
    [SerializeField] private GameObject parent;
    [SerializeField] private GameObject objectToBackTo;

    [Tooltip("Поставить галочку, если текущая панель открывается ВМЕСТО предыдущей, а не просто поверх")]
    [SerializeField] private bool previousObjectIsRequired;

    public void GoBack()
    {
        if (previousObjectIsRequired)
        {
            if (objectToBackTo != null)
            {
                objectToBackTo.SetActive(true);
            }
            else
            {
                Debug.LogError("Object to go back to is not assigned!");
            }
        }

        parent.SetActive(false);
    }
}
