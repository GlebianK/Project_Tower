using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;

public class TutorialCanvas : MonoBehaviour
{
    public static TutorialCanvas Instance { get; private set; }

    [SerializeField] private GameObject[] tutorialPanelsCollection;

    private Dictionary<int, GameObject> tutorialsDict;
    private GameObject activeTutorialPanel;
    private int activeTutorialPanelID;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        activeTutorialPanel = null;
        activeTutorialPanelID = 0;
        tutorialsDict = InitializeTutorialDictionary();

        if (tutorialsDict == null || tutorialsDict.Count < 1)
            Debug.LogWarning("Tutorial panels dictionary is empty!");

        HideAllTutorialPanels();
    }

    private Dictionary<int, GameObject> InitializeTutorialDictionary()
    {
        Dictionary<int, GameObject> tempDict = new();

        if (tutorialPanelsCollection != null
            && tutorialPanelsCollection.Length > 0)
        {
            foreach(GameObject panel in tutorialPanelsCollection)
            {
                if (panel != null && panel.TryGetComponent<TutorialPanel>(out TutorialPanel tpComponent))
                {
                    tempDict.Add(tpComponent.GetTutorialPanelID(), panel);
                }
            }
            Debug.Log("Tutorial collection initiated!");
        }

        return tempDict;
    }

    private void HideAllTutorialPanels()
    {
        if (tutorialPanelsCollection != null
            && tutorialPanelsCollection.Length > 0)
        {
            foreach (GameObject panel in tutorialPanelsCollection)
                panel.SetActive(false);
        }
    }

    public void ShowTutorial(int id)
    {
        HideAllTutorialPanels();
        if (tutorialPanelsCollection != null
            && tutorialPanelsCollection.Length > 0)
        {
            if (tutorialsDict.ContainsKey(id))
            {
                Debug.Log($"Dict contains id {id}, activating panel!");
                if (tutorialsDict.TryGetValue(id, out GameObject panel))
                {
                    panel.SetActive(true);
                    activeTutorialPanel = panel;
                    activeTutorialPanelID = id;
                    Debug.Log("Active!");
                }
            }    
        }
    }

    public void HideTutorial()
    {
        if (activeTutorialPanel != null)
        {
            activeTutorialPanel.SetActive(false);
            activeTutorialPanel = null;
        }
    }

    public void HideTutorial(int tutorialID)
    {
        if (activeTutorialPanelID == tutorialID)
            activeTutorialPanel.SetActive(false);
    }
}
