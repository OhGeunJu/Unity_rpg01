using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    [Header("End screen")]
    [SerializeField] private UI_FadeScreen fadeScreen;
    [SerializeField] private GameObject endText;
    [SerializeField] private GameObject restartButton;

    [Header("Win screen")]
    [SerializeField] private GameObject winText;
    [SerializeField] private GameObject clearButton;

    [Space]

    [Header("UI Groups")]
    [SerializeField] private GameObject charcaterUI;
    [SerializeField] private GameObject InventoryUI;
    [SerializeField] private GameObject skillTreeUI;
    [SerializeField] private GameObject optionsUI;
    [SerializeField] private GameObject inGameUI;
    [SerializeField] private GameObject craftUI;
    [SerializeField] private GameObject stashUI;
    [SerializeField] private GameObject bossUI;

    public UI_SkillToolTip skillToolTip;
    public UI_ItemTooltip itemToolTip;
    public UI_StatToolTip statToolTip;
    public UI_StatToolTip invenStatToolTip;
    public UI_CraftWindow craftWindow;

    public bool isUIOpen = false;

    [Header("Volume Sliders (UI Only)")]
    [SerializeField] private UI_VolumeSlider[] volumeSettings;

    private void Awake()
    {
        // 스킬트리 초기 비활성 처리
        skillTreeUI.SetActive(true);
        skillTreeUI.SetActive(false);

        // 페이드 스크린은 반드시 활성 상태여야 함
        fadeScreen.gameObject.SetActive(true);
    }

    private void Start()
    {
        // 기본 UI 오픈
        SwitchTo(inGameUI);

        bossUI.gameObject.SetActive(false);

        itemToolTip.gameObject.SetActive(false);
        statToolTip.gameObject.SetActive(false);
        skillToolTip.gameObject.SetActive(false);
        invenStatToolTip.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
            SwitchWithKeyTo(charcaterUI);

        if (Input.GetKeyDown(KeyCode.I))
            SwitchWithKeyTo(InventoryUI);

        if (Input.GetKeyDown(KeyCode.K))
            SwitchWithKeyTo(skillTreeUI);

        if (Input.GetKeyDown(KeyCode.O))
            SwitchWithKeyTo(optionsUI);
    }

    public void SwitchTo(GameObject _menu)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            bool isFade = transform.GetChild(i).GetComponent<UI_FadeScreen>() != null;

            if (!isFade)
                transform.GetChild(i).gameObject.SetActive(false);
        }

        if (_menu != null)
        {
            AudioManager.instance.PlaySFX(5, null);
            _menu.SetActive(true);
        }

        // UI 전환 시 게임 정지 여부 결정
        if (GameManager.instance != null)
        {
            if (_menu == inGameUI)
                GameManager.instance.PauseGame(false);
            else
                GameManager.instance.PauseGame(true);
        }
    }

    public void SwitchWithKeyTo(GameObject _menu)
    {
        if (_menu != null && _menu.activeSelf)
        {
            _menu.SetActive(false);
            CheckForInGameUI();
            return;
        }

        SwitchTo(_menu);
    }

    private void CheckForInGameUI()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject.activeSelf &&
                transform.GetChild(i).GetComponent<UI_FadeScreen>() == null)
                return;
        }

        SwitchTo(inGameUI);
    }

    // 엔딩 화면
    public void SwitchOnEndScreen()
    {
        fadeScreen.FadeOut();
        StartCoroutine(EndScreenCorutione());
    }

    IEnumerator EndScreenCorutione()
    {
        yield return new WaitForSeconds(1);
        endText.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        restartButton.SetActive(true);
    }

    public void RestartGameButton()
        => GameManager.instance.RestartScene();

    // 승리 화면
    public void SwitchOnWinScreen()
    {
        fadeScreen.FadeOut();
        StartCoroutine(WinScreenCoroutine());
    }

    private IEnumerator WinScreenCoroutine()
    {
        yield return new WaitForSeconds(1f);
        winText.SetActive(true);          // 승리 텍스트 표시
        yield return new WaitForSeconds(1.5f);
        clearButton.SetActive(true);      // 클리어 버튼 표시
    }

    public void ClearGameButton()
    {
        SaveManager.Instance.SaveGame();

        SceneManager.LoadScene("MainMenu");
    }

    // 크래프트 UI
    public void OpenCraftUI()
    {
        isUIOpen = true;
        SwitchTo(craftUI);
    }

    public void OpenStashUI()
    {
        isUIOpen = true;
        SwitchTo(stashUI);
    }

    public void CloseToInGameUI()
    {
        isUIOpen = false;
        SwitchTo(inGameUI);
    }

    // OptionsSave가 불러온 값을 UI에 적용할 때 호출
    public void ApplyVolumeSliders(float master, float bgm, float sfx)
    {
        foreach (var v in volumeSettings)
        {
            if (v.parametr == SaveKeys.MasterVolume)
                v.LoadSlider(master);

            if (v.parametr == SaveKeys.BGMVolume)
                v.LoadSlider(bgm);

            if (v.parametr == SaveKeys.SFXVolume)
                v.LoadSlider(sfx);
        }
    }
}
