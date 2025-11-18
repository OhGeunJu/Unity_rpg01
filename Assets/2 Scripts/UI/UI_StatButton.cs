using System.Collections;
using System.Collections.Generic;
using UnityEditor.Playables;
using UnityEngine;

public class UI_StatButton : MonoBehaviour
{
    [SerializeField] private StatType statType;          // 이 버튼이 올릴 스탯
    [SerializeField] private UI_StatSlot[] slotsToUpdate; // 갱신해야 할 슬롯들

    private PlayerStats playerStats;

    private void Start()
    {
        playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();
    }

    public void OnClickAllocate()
    {
        playerStats.AllocateStatPoint(statType);

        // 직렬화해 둔 슬롯들만 갱신
        foreach (var slot in slotsToUpdate)
        {
            if (slot != null)
                slot.UpdateStatValueUI();
        }
    }
}
