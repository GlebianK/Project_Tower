using UnityEngine;

public class BackButton : MonoBehaviour
{
    [SerializeField] private GameObject parent;
    [SerializeField] private GameObject objectToBackTo;

    [Tooltip("��������� �������, ���� ������� ������ ����������� ������ ����������, � �� ������ ������")]
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
