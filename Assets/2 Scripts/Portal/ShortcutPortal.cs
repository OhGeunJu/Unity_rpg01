using UnityEngine;

public class ShortcutPortal : Object_NPC, IIteractable
{
    [SerializeField] private KeyCode interactKey = KeyCode.F;
    [SerializeField] private UI_Stage stageSelectUI;

    private bool isLoading = false;

    protected override void Update()
    {
        base.Update();

        Interact();

        Close();
    }

    public void Interact()
    {
        if (!isPlayerInRange || isLoading)
            return;

        if (Input.GetKeyDown(interactKey))
        {
            OnTalk();
        }
    }
    private void Close()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.Escape))
        {
            OnEndTalk();
        }
    }

    public override void OnTalk()
    {
        if (stageSelectUI == null)
        {
            Debug.LogError("ShortcutPortal: StageSelectUI가 설정되지 않았습니다.");
            return;
        }

        // UI 열기 + 콜백 등록
        stageSelectUI.Open(OnStageSelected);
    }

    private void OnStageSelected(StageType stageType)
    {
        string sceneName = StageSceneTable.GetSceneName(stageType);
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError($"ShortcutPortal: StageType {stageType} 에 해당하는 씬 이름이 없습니다.");
            return;
        }

        SaveManager.Instance.SaveGame();

        isLoading = true;
        LoadingSceneLoader.LoadScene(sceneName);
    }
}
