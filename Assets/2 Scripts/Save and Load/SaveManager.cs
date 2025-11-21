using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    [SerializeField] private PlayerStatsSave playerStats;
    [SerializeField] private SkillTreeSave skillTree;
    [SerializeField] private InventorySave inventory;
    [SerializeField] private CheckpointSave checkpoints;
    [SerializeField] private OptionsSave options;

    public OptionsSave Options => options;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void RegisterPlayerStats(PlayerStatsSave s) => playerStats = s;
    public void RegisterSkillTree(SkillTreeSave s) => skillTree = s;
    public void RegisterInventory(InventorySave s) => inventory = s;
    public void RegisterCheckpoints(CheckpointSave s) => checkpoints = s;
    public void RegisterOptions(OptionsSave s) => options = s;

    /// <summary>새 게임 시작: 기존 세이브를 날리고 기본값으로 초기화</summary>
    public void NewGame()
    {
        DeleteSavedData();

        if (playerStats != null) playerStats.ResetToDefault();
        if (skillTree != null) skillTree.ResetToDefault();
        if (inventory != null) inventory.ResetToDefault();
        if (checkpoints != null) checkpoints.ResetToDefault();
        if (options != null) options.ResetToDefault();

        // 각 시스템의 기본 상태를 세팅
        // 예: PlayerStats 기본값, 스킬 초기 상태, 인벤토리 빈 상태 등
        // 필요하다면 여기서 각 매니저의 Init() 호출
    }

    /// <summary>세이브 데이터가 존재하는지 여부</summary>
    public bool HasSavedData()
    {
        foreach (var key in SaveKeys.AllKeys)
        {
            if (ES3.KeyExists(key))
                return true;
        }
        return false;
    }

    /// <summary>게임 전체 저장</summary>
    public void SaveGame()
    {
        if (playerStats != null) playerStats.Save();
        if (skillTree != null) skillTree.Save();
        if (inventory != null) inventory.Save();
        if (checkpoints != null) checkpoints.Save();
        if (options != null) options.Save();
    }

    /// <summary>게임 전체 불러오기</summary>
    public void LoadGame()
    {
        if (!HasSavedData())
        {
            Debug.Log("저장된 데이터가 없어 NewGame으로 초기화합니다.");
            NewGame();
            return;
        }

        if (playerStats != null) playerStats.Load();
        if (skillTree != null) skillTree.Load();
        if (inventory != null) inventory.Load();
        if (checkpoints != null) checkpoints.Load();
        if (options != null) options.Load();

        SkillManager.instance.RefreshSkillUnlocks();
    }

    /// <summary>전체 세이브 데이터 삭제</summary>
    [ContextMenu("Delete save file")]
    public void DeleteSavedData()
    {
        // SaveKeys에 정의된 모든 키를 삭제
        foreach (var key in SaveKeys.AllKeys)
        {
            if (ES3.KeyExists(key))
                ES3.DeleteKey(key);
        }
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }
}
