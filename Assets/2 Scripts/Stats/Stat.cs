using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat // 스탯 클래스
{
   [SerializeField] private int baseValue; // 기본 값

    public List<int> modifiers;
    public int GetValue() // 스탯의 최종 값을 계산하는 메서드
    {
        int finalValue = baseValue; 

        foreach (int modifier in modifiers)
        {
            finalValue += modifier;
        }

        return finalValue;
    }

    public void SetDefaultValue(int _value) // 기본 값 설정 메서드
    {
        baseValue = _value;
    }

    public void AddModifier(int _modifier) // 수정자 추가 메서드
    {
        modifiers.Add(_modifier);
    }

    public void RemoveModifier(int _modifier) // 수정자 제거 메서드
    {
        modifiers.Remove(_modifier);
    }
}
