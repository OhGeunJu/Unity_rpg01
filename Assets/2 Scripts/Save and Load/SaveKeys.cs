public static class SaveKeys
{
    public const string Currency = "Currency";

    public const string PlayerLevel = "PlayerLevel";
    public const string PlayerExp = "PlayerExp";
    public const string PlayerStatPoints = "PlayerStatPoints";

    public const string StatStrength = "StatStrength";
    public const string StatAgility = "StatAgility";
    public const string StatIntelligence = "StatIntelligence";
    public const string StatVitality = "StatVitality";


    public const string SkillTree = "SkillTree";        // Dictionary<string, bool>
    public const string Inventory = "Inventory";        // Dictionary<string, int>
    public const string EquipmentIds = "EquipmentIds";     // List<string>

    public const string CheckpointDict = "CheckpointDict";      // Dictionary<string, bool>
    public const string ClosestCheckpointId = "ClosestCheckpointId";

    public const string LostCurrencyX = "LostCurrencyX";
    public const string LostCurrencyY = "LostCurrencyY";
    public const string LostCurrencyAmount = "LostCurrencyAmount";

    public const string VolumeSettings = "VolumeSettings";   // Dictionary<string, float>
    public const string MasterVolume = "MasterVolume";
    public const string BGMVolume = "BGMVolume";
    public const string SFXVolume = "SFXVolume";
    public const string HpBarToggle = "HpBarToggle";

    // 전체 삭제에 쓸 키 목록
    public static readonly string[] AllKeys = new[]
    {
        Currency,
        PlayerLevel,
        PlayerExp,
        PlayerStatPoints,
        StatStrength,
        StatAgility,
        StatIntelligence,
        StatVitality,
        SkillTree,
        Inventory,
        EquipmentIds,
        CheckpointDict,
        ClosestCheckpointId,
        LostCurrencyX,
        LostCurrencyY,
        LostCurrencyAmount,
        VolumeSettings,
        SFXVolume,
        BGMVolume,
        MasterVolume,
        HpBarToggle
    };
}
