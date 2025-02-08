using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthView : MonoBehaviour
{
    [SerializeField] private Health healthToView;
    [SerializeField] private Slider hpBar;
    [SerializeField] private TMP_Text hpText;

    private void OnEnable()
    {
        healthToView.TookDamage.AddListener(OnHealthChanged);
        healthToView.Healed.AddListener(OnHealthChanged);
    }

    private void OnDisable()
    {
        healthToView.TookDamage.RemoveListener(OnHealthChanged);
        healthToView.Healed.RemoveListener(OnHealthChanged);
    }

    private void Start()
    {
        OnHealthChanged();
    }

    private void OnHealthChanged()
    {
        if (hpBar != null)
        {
            float hpClamp01 = healthToView.HP / healthToView.MaxHP;
            hpBar.value = hpClamp01;
        }

        if (hpText != null)
        {
            hpText.text = $"{Mathf.FloorToInt(healthToView.HP)}";
        }
    }
}
