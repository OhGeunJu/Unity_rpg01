using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_SkillTreeSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private UI ui;
    private Image skillImage;

    [SerializeField] private int skillCost;
    [SerializeField] private string skillName;
    [TextArea]
    [SerializeField] private string skillDescription;
    [SerializeField] private Color lockedSkillColor;

    [Header("Unlock Conditions")]
    public bool unlocked;

    [SerializeField] private UI_SkillTreeSlot[] shouldBeUnlocked;
    [SerializeField] private UI_SkillTreeSlot[] shouldBeLocked;

    // ─────────────────────────────────────────────
    // SkillTreeSave에서 사용할 공개 프로퍼티/메서드
    // ─────────────────────────────────────────────
    public string SkillId => skillName;
    public bool IsUnlocked => unlocked;

    public void SetUnlocked(bool value)
    {
        unlocked = value;

        if (skillImage == null)
            skillImage = GetComponent<Image>();

        skillImage.color = unlocked ? Color.white : lockedSkillColor;
    }
    // ─────────────────────────────────────────────

    private void OnValidate()
    {
        gameObject.name = "SkillTreeSlot_UI - " + skillName;
    }

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() => UnlockSkillSlot());
    }

    private void Start()
    {
        skillImage = GetComponent<Image>();
        ui = GetComponentInParent<UI>();

        // 시작 상태 반영
        SetUnlocked(unlocked);
    }

    public void UnlockSkillSlot()
    {
        // 돈 부족하면 패스
        if (PlayerManager.instance.HaveEnoughMoney(skillCost) == false)
            return;

        // 선행 스킬이 안 열려 있으면 패스
        for (int i = 0; i < shouldBeUnlocked.Length; i++)
        {
            if (shouldBeUnlocked[i] != null && shouldBeUnlocked[i].unlocked == false)
            {
                Debug.Log("Cannot unlock skill");
                return;
            }
        }

        // 같이 열려 있으면 안 되는 스킬이 이미 열려 있으면 패스
        for (int i = 0; i < shouldBeLocked.Length; i++)
        {
            if (shouldBeLocked[i] != null && shouldBeLocked[i].unlocked == true)
            {
                Debug.Log("Cannot unlock skill");
                return;
            }
        }

        unlocked = true;
        SetUnlocked(true);

        // 여기서 SkillTreeSave에 알리고 싶으면,
        // FindObjectOfType<SkillTreeSave>() 같은 걸로 한 번 알려도 됨
        // (나중에 필요하면 추가)
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ui.skillToolTip.ShowToolTip(skillDescription, skillName, skillCost);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ui.skillToolTip.HideToolTip();
    }
}
