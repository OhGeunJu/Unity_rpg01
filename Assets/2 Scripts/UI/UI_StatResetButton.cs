using System.Collections.Generic;
using UnityEngine;

public class UI_StatResetButton : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;

    private void Awake()
    {
        // 인스펙터에서 안 넣었을 때 대비
        if (playerStats == null && PlayerManager.instance != null && PlayerManager.instance.player != null)
        {
            playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();
        }
    }

    // 버튼 OnClick에 이 함수 연결
    public void OnClickReset()
    {
        if (playerStats == null)
        {
            Debug.LogWarning("[UI_StatResetButton] PlayerStats가 설정되어 있지 않습니다.");
            return;
        }

        playerStats.ResetPoint();
    }
}
