using UnityEngine;

public enum StageType
{
    None = 0,
    Stage1 = 1,
    Stage2 = 2,
    BossRoom = 3,
    Village = 4
}

public static class StageSceneTable
{
    public static string GetSceneName(StageType stageType)
    {
        switch (stageType)
        {
            case StageType.Stage1:
                return "Stage 1";
            case StageType.Stage2:
                return "Stage 2";
            case StageType.BossRoom:
                return "BossRoom";
            case StageType.Village:
                return "Village";


            default:
                Debug.LogError($"정의되지 않은 StageType: {stageType}");
                return string.Empty;
        }
    }
}

