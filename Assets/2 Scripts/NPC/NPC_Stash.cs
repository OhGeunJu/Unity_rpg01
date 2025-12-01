using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Stash : Object_NPC, IIteractable
{
    [SerializeField] private KeyCode interactKey = KeyCode.F;

    protected override void Update()
    {
        base.Update();

        Interact();

        Close();
    }

    private void Close()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.Escape))
        {
            OnEndTalk();
        }
    }

    public void Interact()
    {
        if (isPlayerInRange && Input.GetKeyDown(interactKey))
        {
            OnTalk();
        }
    }
    public override void OnTalk()
    {
        if(ui.isUIOpen)
            ui.CloseToInGameUI();
        else
            ui.OpenStashUI();
    }
}
