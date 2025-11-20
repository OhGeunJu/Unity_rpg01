using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsSave : MonoBehaviour
{
    /// <summary>채널 이름(예: "Master", "BGM", "SFX") → 볼륨(0~1)</summary>
    private Dictionary<string, float> volumeSettings = new Dictionary<string, float>();

    [Header("Gameplay UI Settings")]
    [SerializeField] private bool hpBarOn = true;

    public IReadOnlyDictionary<string, float> VolumeSettings => volumeSettings;

    public void SetVolume(string channel, float value)
    {
        if (string.IsNullOrEmpty(channel))
            return;

        volumeSettings[channel] = Mathf.Clamp01(value);
        // 여기에 오디오 믹서 반영 코드 추가 가능
    }

    public float GetVolume(string channel, float defaultValue = 1f)
    {
        if (volumeSettings.TryGetValue(channel, out float v))
            return v;

        return defaultValue;
    }

    public void Save()
    {
        ES3.Save(SaveKeys.VolumeSettings, volumeSettings);
        ES3.Save(SaveKeys.HpBarToggle, hpBarOn);
    }

    public void Load()
    {
        volumeSettings = ES3.Load(SaveKeys.VolumeSettings, new Dictionary<string, float>());
        hpBarOn = ES3.Load<bool>(SaveKeys.HpBarToggle, true);
        ApplyHpBar();

        // 불러온 후 오디오 시스템에 바로 반영하고 싶으면 여기서 처리
    }

    public void ResetToDefault()
    {
        volumeSettings.Clear();
        hpBarOn = true;
        // 필요하면 기본 볼륨 값 세팅
        // volumeSettings["Master"] = 1f;
        // volumeSettings["BGM"] = 1f;
        // volumeSettings["SFX"] = 1f;
    }
    public void ApplyHpBar()
    {
        // UI_HpBarToggle는 Unity UI Toggle 컴포넌트와 연결되어 있어야 한다면:
        var toggle = FindObjectOfType<UI_HpBarToggle>();
        if (toggle != null)
        {
            toggle.GetComponent<Toggle>().isOn = hpBarOn;
            toggle.OnToggleHpBar(hpBarOn);
        }
    }

    public void SetHpBar(bool isOn)
    {
        hpBarOn = isOn;
        Save();       // 즉시 저장
    }
}
