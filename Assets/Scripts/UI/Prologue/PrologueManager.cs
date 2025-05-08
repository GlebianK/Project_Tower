using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class PrologueManager : MonoBehaviour
{
    [SerializeField] private GameObject taskPanelObject;

    [SerializeField] private ProloguePanel panelObject;
    [SerializeField] private List<ProloguePanelInfo> panels;

    private int panelIndex = 0;
    private bool isCoroutineRun = false;

    private void Start()
    {
        StartCoroutine(ChangePanelCoroutine(true));
    }

    private void ChangePanel()
    {
        StartCoroutine(ChangePanelCoroutine(false));
    }

    private IEnumerator ChangePanelCoroutine(bool isAwake = false)
    {
        isCoroutineRun = true;
        if (!isAwake)
        {
            panelObject.Disappear();
            yield return new WaitForSeconds(1);
        }

        panelObject.Appear(panels[panelIndex], isAwake);
        panelObject.PrevButton.gameObject.SetActive(panelIndex > 0);

        yield return new WaitForSeconds(1);
        ListenEventsOnClick();

        isCoroutineRun = false;
    }

    private void ListenEventsOnClick()
    {
        if (panelIndex > 0)
        {
            panelObject.PrevButton.onClick.AddListener(OnClickBack);
        }
        panelObject.NextButton.onClick.AddListener(OnClickNext);
    }

    private void OnClickBack()
    {
        panelIndex--;
        ChangePanel();
    }
    private void OnClickNext()
    {
        panelIndex++;
        if (panelIndex == panels.Count)
        {
            taskPanelObject.SetActive(true);
            panelObject.gameObject.SetActive(false);    
        }
        else
        {
            ChangePanel();
        }
    }
}
