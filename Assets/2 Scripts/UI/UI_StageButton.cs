using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_StageButton : MonoBehaviour
{
    [SerializeField] private StageType stageType;
    [SerializeField] private UI_Stage stageSelectUI;
    [SerializeField] private Button button;

    [Header("Text 색 설정")]
    [SerializeField] private TextMeshProUGUI stageNameText;
    [SerializeField] private Color lockedTextColor = Color.gray;
    [SerializeField] private Color unlockedTextColor = Color.white;

    public StageType StageType => stageType;

    private void Awake()
    {
        if (button != null)
            button.onClick.AddListener(OnClick);

        // 인스펙터에서 언락 색을 안 넣어놨다면, 현재 색을 기준으로 사용
        if (stageNameText != null && unlockedTextColor == default)
            unlockedTextColor = stageNameText.color;
    }

    private void OnClick()
    {
        if (stageSelectUI != null)
            stageSelectUI.SelectStage(stageType);
    }

    public void SetLocked(bool locked)
    {
        if (button != null)
            button.interactable = !locked;

        if (stageNameText != null)
            stageNameText.color = locked ? lockedTextColor : unlockedTextColor;
    }
}