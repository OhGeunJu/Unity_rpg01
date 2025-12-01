using System.Collections;
using UnityEngine;

public class Portal : Object_NPC, IIteractable
{
    [SerializeField] private KeyCode interactKey = KeyCode.F;
    [SerializeField] private string sceneToLoad;

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

        if (string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.LogError("Portal_Interact: sceneToLoad 가 설정되지 않았습니다.");
            return;
        }

        isLoading = true;

        LoadingSceneLoader.LoadScene(sceneToLoad);
    }
}