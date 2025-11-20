using UnityEngine;

public class PlayerStatsSave : MonoBehaviour
{
    [SerializeField] private PlayerStats stats;

    private void Awake()
    {
        if (stats == null)
            stats = GetComponent<PlayerStats>();
    }

    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    // ÀúÀå
    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    public void Save()
    {
        // ±âº» ½ºÅÈ
        ES3.Save(SaveKeys.PlayerLevel, stats.level);
        ES3.Save(SaveKeys.PlayerExp, stats.Exp);
        ES3.Save(SaveKeys.PlayerStatPoints, stats.statPoints);

        // ´É·ÂÄ¡
        ES3.Save(SaveKeys.StatStrength, stats.strength.GetBase());
        ES3.Save(SaveKeys.StatAgility, stats.agility.GetBase());
        ES3.Save(SaveKeys.StatIntelligence, stats.intelligence.GetBase());
        ES3.Save(SaveKeys.StatVitality, stats.vitality.GetBase());

        // ÀÒÀº È­Æó
        ES3.Save(SaveKeys.LostCurrencyAmount, GameManager.instance.lostCurrencyAmount);
        ES3.Save(SaveKeys.LostCurrencyX, GameManager.instance.lostCurrencyPosition.x);
        ES3.Save(SaveKeys.LostCurrencyY, GameManager.instance.lostCurrencyPosition.y);
    }

    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    // ·Îµå
    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    public void Load()
    {
        // ±âº» ½ºÅÈ
        stats.level = ES3.Load<int>(SaveKeys.PlayerLevel, 1);
        stats.Exp = ES3.Load<int>(SaveKeys.PlayerExp, 0);
        stats.statPoints = ES3.Load<int>(SaveKeys.PlayerStatPoints, 0);

        // ´É·ÂÄ¡
        stats.strength.SetValue(ES3.Load<int>(SaveKeys.StatStrength, 0));
        stats.agility.SetValue(ES3.Load<int>(SaveKeys.StatAgility, 0));
        stats.intelligence.SetValue(ES3.Load<int>(SaveKeys.StatIntelligence, 0));
        stats.vitality.SetValue(ES3.Load<int>(SaveKeys.StatVitality, 0));

        stats.UpdateDerivedStats();

        // ÀÒÀº È­Æó ·Îµå
        GameManager.instance.lostCurrencyAmount =
            ES3.Load<int>(SaveKeys.LostCurrencyAmount, 0);

        float x = ES3.Load<float>(SaveKeys.LostCurrencyX, 0);
        float y = ES3.Load<float>(SaveKeys.LostCurrencyY, 0);
        GameManager.instance.lostCurrencyPosition = new Vector2(x, y);
    }

    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    // »õ °ÔÀÓ ÃÊ±âÈ­
    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    public void ResetToDefault()
    {
        // ½ºÅÈ ÃÊ±âÈ­
        stats.level = 1;
        stats.Exp = 0;
        stats.statPoints = 0;

        stats.strength.SetValue(0);
        stats.agility.SetValue(0);
        stats.intelligence.SetValue(0);
        stats.vitality.SetValue(0);
        stats.UpdateDerivedStats();

        // ÀÒÀº È­Æó ÃÊ±âÈ­
        GameManager.instance.lostCurrencyAmount = 0;
        GameManager.instance.lostCurrencyPosition = Vector2.zero;

        Save();
    }
}
