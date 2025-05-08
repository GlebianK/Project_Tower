using DG.Tweening;
using DG.Tweening.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProloguePanel : MonoBehaviour
{
    [SerializeField] private Animator uiAnimator;
    [SerializeField] private Image background;
    [SerializeField] private TMP_Text text;
    [SerializeField] private Button prevButton;
    [SerializeField] private Button nextButton;

    public Button PrevButton => prevButton;
    public Button NextButton => nextButton;

    public void Appear(ProloguePanelInfo info, bool insta = false)
    {
        //if (insta)
        //{
            uiAnimator.Play("Appear");
        //}
        //else
        //{
        //    uiAnimator.Play("Appear Delay");
        //}

        background.sprite = info.background;
        text.text = info.scenarioText;
    }

    public void Disappear()
    {
        prevButton.onClick.RemoveAllListeners();
        nextButton.onClick.RemoveAllListeners();
        uiAnimator.Play("Disappear");
    }
}
