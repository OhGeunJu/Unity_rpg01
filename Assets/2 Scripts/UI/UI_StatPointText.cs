using TMPro;
using UnityEngine;

public class UI_StatPointText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI statPointText;

    private PlayerStats playerStats;

    private void Awake()
    {
        if (statPointText == null)
            statPointText = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        if (PlayerManager.instance != null && PlayerManager.instance.player != null)
        {
            playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

            if (playerStats != null)
            {
                playerStats.onStatPointChanged += UpdateStatPointText;
                // 현재 값으로 한 번 초기화
                UpdateStatPointText(playerStats.statPoints);
            }
        }
    }

    private void OnDisable()
    {
        if (playerStats != null)
            playerStats.onStatPointChanged -= UpdateStatPointText;
    }

    private void UpdateStatPointText(int currentPoint)
    {
        if (statPointText == null)
            return;

        statPointText.text = currentPoint.ToString();
    }
}
