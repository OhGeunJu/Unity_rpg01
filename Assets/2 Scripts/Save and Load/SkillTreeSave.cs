using System.Collections.Generic;
using UnityEngine;

public class SkillTreeSave : MonoBehaviour
{
    [Header("이 트리 안의 모든 스킬 슬롯들")]
    [SerializeField] private UI_SkillTreeSlot[] slots;

    // ES3에 저장할 실제 데이터: 스킬ID -> 해금 여부
    private Dictionary<string, bool> skillTree = new Dictionary<string, bool>();

    private void Awake()
    {
        // 인스펙터에 안 넣어줬으면 자식에서 자동으로 찾기
        if (slots == null || slots.Length == 0)
            slots = GetComponentsInChildren<UI_SkillTreeSlot>(true);

        SaveManager.Instance.RegisterSkillTree(this);
    }

    /// <summary>ES3에 스킬 트리 상태 저장</summary>
    public void Save()
    {
        skillTree.Clear();

        foreach (var slot in slots)
        {
            if (slot == null || string.IsNullOrEmpty(slot.SkillId))
                continue;

            skillTree[slot.SkillId] = slot.IsUnlocked;
        }

        ES3.Save(SaveKeys.SkillTree, skillTree);
    }

    /// <summary>ES3에서 스킬 트리 상태 불러오기</summary>
    public void Load()
    {
        skillTree = ES3.Load<Dictionary<string, bool>>(
            SaveKeys.SkillTree,
            new Dictionary<string, bool>()
        );

        foreach (var slot in slots)
        {
            if (slot == null || string.IsNullOrEmpty(slot.SkillId))
                continue;

            bool unlocked = false;
            skillTree.TryGetValue(slot.SkillId, out unlocked);
            slot.SetUnlocked(unlocked);
        }
    }

    /// <summary>새 게임용 기본 상태</summary>
    public void ResetToDefault()
    {
        foreach (var slot in slots)
        {
            if (slot == null)
                continue;

            // 필요하다면 "기본으로 열려 있는 스킬" 설정 추가 가능
            slot.SetUnlocked(false);
        }

        Save();
    }

    /// <summary>외부에서 스킬 해금할 때 호출하기 좋은 헬퍼</summary>
    public void UnlockSkill(string skillId)
    {
        if (string.IsNullOrEmpty(skillId))
            return;

        foreach (var slot in slots)
        {
            if (slot != null && slot.SkillId == skillId)
            {
                slot.SetUnlocked(true);
                break;
            }
        }

        Save();
    }
}
