using UnityEngine;
using UnityEngine.UI;

public class UI_HpBarToggle : MonoBehaviour
{
    [SerializeField] private EntityFX entityFx;    // 플레이어 쪽 EntityFX
    [SerializeField] private GameObject hpBarRoot; // 실제 체력바 오브젝트 (지금 SetActive 걸어둔 그거)

    // Toggle의 On Value Changed(bool) 에 연결할 함수
    public void OnToggleHpBar(bool isOn)
    {
        // 1) 체력바 오브젝트 켜고 끄기
        if (hpBarRoot != null)
            hpBarRoot.SetActive(isOn);

        // 2) EntityFX 쪽에 "유저가 HpBar 켜둔 상태"를 알려주기
        if (entityFx != null)
            entityFx.SetUserHpBarState(isOn);

        // ★ 옵션 저장 호출
        if (SaveManager.Instance != null && SaveManager.Instance.Options != null)
        {
            SaveManager.Instance.Options.SetHpBar(isOn);
        }
    }
}