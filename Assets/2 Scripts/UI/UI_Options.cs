using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Options : MonoBehaviour
{
    public void OnSaveButton()
    {
        SaveManager.Instance.SaveGame();
    }
}
