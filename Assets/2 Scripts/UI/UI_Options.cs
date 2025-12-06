using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Options : MonoBehaviour
{
    public void OnSaveAndExitButton()
    {
        StartCoroutine(SaveAndQuit());
    }

    private IEnumerator SaveAndQuit()
    {
        SaveManager.Instance.SaveGame();

        yield return new WaitForSecondsRealtime(0.2f);

        Application.Quit();
    }
}
