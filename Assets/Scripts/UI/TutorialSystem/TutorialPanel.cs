using UnityEngine;

public class TutorialPanel : MonoBehaviour
{
    [SerializeField] private int tutorialPanelID;

    [Tooltip("Опциональное поле для названия инстанса туториала, чтобы проще было остлеживать")]
    [SerializeField] private string tutorialName;

    public int GetTutorialPanelID() => tutorialPanelID;

    private void Awake()
    {
        gameObject.SetActive(false);
    }
}
