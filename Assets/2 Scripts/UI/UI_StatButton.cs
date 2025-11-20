using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_StatButton : MonoBehaviour
{
    [SerializeField] private StatType statType;          // 이 버튼이 올릴 스탯

    private PlayerStats playerStats;

    private void Start()
    {
        playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();
    }

    public void OnClickAllocate()
    {
        playerStats.AllocateStatPoint(statType);
    }
}
