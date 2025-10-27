using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Boss : MonoBehaviour
{
    [SerializeField] private EnemyStats BossStats;
    [SerializeField] private Slider slider;

    void Start()
    {
        if (BossStats != null)
            BossStats.onHealthChanged += UpdateHealthUI;
    }

    private void UpdateHealthUI()
    {
        slider.maxValue = BossStats.GetMaxHealthValue();
        slider.value = BossStats.currentHealth;
    }
}
