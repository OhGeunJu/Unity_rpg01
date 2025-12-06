using System;
using System.Collections.Generic;
using UnityEngine;

public class UI_Stage : MonoBehaviour
{
    [SerializeField] private GameObject root; // 전체 패널
    [SerializeField] private List<UI_StageButton> stageButtons;

    private Action<StageType> onStageSelected;

    private void Awake()
    {
        // 시작 시에는 닫아두기
        if (root != null)
            root.SetActive(false);
    }

    public void Open(Action<StageType> onSelected)
    {
        onStageSelected = onSelected;

        if (root != null)
            root.SetActive(true);

        RefreshButtons();
    }

    public void Close()
    {
        if (root != null)
            root.SetActive(false);
    }

    private void RefreshButtons()
    {
        if (stageButtons == null)
            return;

        foreach (var btn in stageButtons)
        {
            if (btn == null)
                continue;

            bool unlocked = false;

            // 1) 무조건 열려 있어야 하는 스테이지는 여기서 바로 처리
            if (btn.StageType == StageType.Village)
            {
                unlocked = true;
            }
            // 2) 나머지는 SaveManager/StageProgress가 있을 때만 세이브 기준으로 판단
            else if (SaveManager.Instance != null &&
                     SaveManager.Instance.StageProgress != null)
            {
                unlocked = SaveManager.Instance.StageProgress.IsStageUnlocked(btn.StageType);
            }
            else
            {
                // 여기로 떨어지면 SaveManager 쪽이 아직 준비 안 된 상태
                // 개발 중 확인하려면 로그를 잠깐 찍어봐도 좋습니다.
                Debug.LogWarning($"Stage UI: SaveManager 또는 StageProgress가 아직 null입니다. {btn.StageType}를 잠금으로 처리합니다.");
                unlocked = false;
            }

            btn.SetLocked(!unlocked);
        }
    }

    public void SelectStage(StageType stageType)
    {
        // 잠금된 스테이지면 막기
        if (SaveManager.Instance != null &&
            SaveManager.Instance.StageProgress != null &&
            !SaveManager.Instance.StageProgress.IsStageUnlocked(stageType))
        {
            Debug.Log($"StageSelectUI: 잠금된 스테이지입니다. ({stageType})");
            return;
        }

        // StageSceneTable에서 실제 씬 이름 가져오기
        string sceneName = StageSceneTable.GetSceneName(stageType);

        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError($"StageSelectUI: {stageType} 씬 이름이 올바르게 설정되지 않았습니다.");
            return;
        }

        // 선택 콜백 필요 시 실행
        onStageSelected?.Invoke(stageType);

        // ▶ 로딩씬 → 실제 스테이지 씬 전환
        LoadingSceneLoader.LoadScene(sceneName);

        Close();
    }
}
