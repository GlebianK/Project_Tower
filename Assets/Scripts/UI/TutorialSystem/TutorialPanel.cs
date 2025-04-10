using UnityEngine;

public class TutorialPanel : MonoBehaviour
{
    [SerializeField] private int tutorialPanelID;

    [Tooltip("������������ ���� ��� �������� �������� ���������, ����� ����� ���� �����������")]
    [SerializeField] private string tutorialName;

    public int GetTutorialPanelID() => tutorialPanelID;

    private void Awake()
    {
        gameObject.SetActive(false);
    }
}
