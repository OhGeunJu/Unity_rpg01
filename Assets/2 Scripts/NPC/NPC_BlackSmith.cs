using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_BlackSmith : Object_NPC, IIteractable
{
    public void Interact()
    {
        Debug.Log("Open BlackSmith shop");
    }

}
