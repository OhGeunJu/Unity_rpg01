using System.Collections;
using UnityEngine;

public class Portal : Object_NPC, IIteractable
{
    [SerializeField] private KeyCode interactKey = KeyCode.F;

    [SerializeField] private bool unlockStageOnUse = false;
    [SerializeField] private StageType stageToUnlock = StageType.None;

    private bool isLoading = false;

    protected override void Update()
    {
        base.Update();

        Interact();
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

    public override void OnTalk()
    {

        if (stageToUnlock == StageType.None)
        {
            Debug.LogError("Portal: StageType을 설정하세요.");
            return;
        }


        TryUnlockStage();

        SaveManager.Instance.SaveGame();

        isLoading = true;

        string sceneName = StageSceneTable.GetSceneName(stageToUnlock);

        LoadingSceneLoader.LoadScene(sceneName);
    }

    private void TryUnlockStage()
    {
        if (!unlockStageOnUse)
            return;

        if (SaveManager.Instance == null || SaveManager.Instance.StageProgress == null)
            return;

        if (stageToUnlock == StageType.None)
            return;

        SaveManager.Instance.StageProgress.UnlockStage(stageToUnlock);
    }
}