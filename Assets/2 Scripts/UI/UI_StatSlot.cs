using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_StatSlot : MonoBehaviour , IPointerEnterHandler, IPointerExitHandler
{
    private UI ui;
    private PlayerStats playerStats;

    [SerializeField] private string statName;
    [SerializeField] private StatType statType;
    [SerializeField] private TextMeshProUGUI statValueText;
    [SerializeField] private TextMeshProUGUI statNameText;

    [TextArea]
    [SerializeField] private string statDescription;

    private void Awake()
    {
        ui = FindObjectOfType<UI>();
    }
    private void OnValidate()
    {
        gameObject.name = "Stat - " + statName;


        if(statNameText != null)
            statNameText.text = statName;
    }
   
    private void OnEnable()
    {
        // PlayerManager / player가 준비 안 됐으면 그냥 경고만 찍고 리턴
        if (PlayerManager.instance == null || PlayerManager.instance.player == null)
        {
#if UNITY_EDITOR
            Debug.LogWarning($"[UI_StatSlot] {name} : PlayerManager / Player가 아직 준비 안 됨");
#endif
            return;
        }

        playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();
        if (playerStats == null)
        {
#if UNITY_EDITOR
            Debug.LogWarning($"[UI_StatSlot] {name} : PlayerStats를 찾지 못함");
#endif
            return;
        }

        playerStats.onStatsChanged += HandleStatsChanged;
        HandleStatsChanged();   // 첫 값 갱신
    }

    private void OnDisable()
    {
        if (playerStats != null)
            playerStats.onStatsChanged -= HandleStatsChanged;
    }

    private void HandleStatsChanged()
    {
        UpdateStatValueUI();
    }

    public void UpdateStatValueUI()
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        if(playerStats != null)
        {
            statValueText.text = playerStats.GetStat(statType).GetValue().ToString();



            if (statType == StatType.health)
                statValueText.text = playerStats.GetMaxHealthValue().ToString();

            if (statType == StatType.damage)
                statValueText.text = (playerStats.damage.GetValue() + playerStats.strength.GetValue()).ToString();

            if (statType == StatType.critPower)
                statValueText.text = (playerStats.critPower.GetValue() + playerStats.strength.GetValue()).ToString();

            if(statType == StatType.critChance)
                statValueText.text = (playerStats.critChance.GetValue() + playerStats.agility.GetValue()).ToString();

            if (statType == StatType.evasion)
                statValueText.text = (playerStats.evasion.GetValue() + playerStats.agility.GetValue()).ToString();

            if (statType == StatType.magicRes)
                statValueText.text = (playerStats.magicResistance.GetValue() + (playerStats.intelligence.GetValue() * 3)).ToString();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (ui.statToolTip != null)
            ui.statToolTip.ShowStatToolTip(statDescription);
        if (ui.invenStatToolTip != null)
            ui.invenStatToolTip.ShowStatToolTip(statDescription);
    }
    

    public void OnPointerExit(PointerEventData eventData)
    {
        if (ui.statToolTip != null)
            ui.statToolTip.HideStatToolTip();
        if (ui.invenStatToolTip != null)
            ui.invenStatToolTip.HideStatToolTip();
    }
}
