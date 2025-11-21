using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class UI_VolumeSlider : MonoBehaviour
{
    public Slider slider;

    public string parametr;
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private float multiplier= 20f;

    public void SliderValue(float value)
    {
        // 0~1 값을 dB로 변환 (로그 스케일)
        float v = Mathf.Clamp(value, 0.0001f, 1f);
        float dB = Mathf.Log10(v) * multiplier;

        audioMixer.SetFloat(parametr, dB);
    }

    public void LoadSlider(float value)
    {
        if (slider == null)
            return;

        slider.value = value;
    }

}
