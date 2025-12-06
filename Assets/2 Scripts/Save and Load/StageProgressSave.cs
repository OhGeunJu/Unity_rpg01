using System.Collections.Generic;
using UnityEngine;

public class StageProgressSave : MonoBehaviour
{
    [SerializeField]
    private List<StageType> unlockedStages = new List<StageType>();

    private const string SaveKey = SaveKeys.StageProgress;

    /// <summary>새 게임 기준 기본 해금 상태</summary>
    public void ResetToDefault()
    {
        unlockedStages.Clear();

        // 처음부터 열려 있을 스테이지만 추가
        // 예: Stage1만 기본 오픈
        unlockedStages.Add(StageType.Village);

        Save(); // 원하시면 여기서 바로 저장, 아니면 생략 가능
    }

    public void Save()
    {
        // ES3가 enum 리스트를 바로 저장해도 되지만,
        // 안전하게 int 리스트로 변환해서 저장하는 방식으로 갑니다.
        List<int> ids = new List<int>();
        foreach (var s in unlockedStages)
            ids.Add((int)s);

        ES3.Save(SaveKey, ids);
    }

    public void Load()
    {
        if (!ES3.KeyExists(SaveKey))
        {
            // 세이브가 없으면 기본값으로 초기화
            ResetToDefault();
            return;
        }

        List<int> ids = ES3.Load<List<int>>(SaveKey);

        unlockedStages.Clear();

        if (ids != null)
        {
            foreach (int id in ids)
            {
                if (System.Enum.IsDefined(typeof(StageType), id))
                {
                    var stage = (StageType)id;
                    if (!unlockedStages.Contains(stage))
                        unlockedStages.Add(stage);
                }
            }
        }
    }

    public bool IsStageUnlocked(StageType stage)
    {
        if (stage == StageType.Village)
            return true;

        return unlockedStages.Contains(stage);
    }

    public void UnlockStage(StageType stage)
    {
        if (stage == StageType.None)
            return;

        if (!unlockedStages.Contains(stage))
        {
            unlockedStages.Add(stage);
            Save();
        }
    }

    public List<StageType> GetUnlockedStages()
    {
        return new List<StageType>(unlockedStages);
    }
}
