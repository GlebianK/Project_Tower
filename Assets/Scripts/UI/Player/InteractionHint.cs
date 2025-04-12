using TMPro;
using UnityEngine;

public class InteractionHint : MonoBehaviour
{
    [SerializeField] private TMP_Text pickUpHintText;

    private void Awake()
    {
        HideHint();
    }

    public void ShowHint()
    {
        pickUpHintText.gameObject.SetActive(true);
    }

    public void HideHint()
    {
        pickUpHintText.gameObject.SetActive(false);
    }
}
