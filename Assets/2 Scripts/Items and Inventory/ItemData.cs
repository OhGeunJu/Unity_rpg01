using System.Text;
using UnityEngine;




#if UNITY_EDITOR
using UnityEditor;
#endif

public enum ItemType // 아이템 타입
{
    Material, // 재료
    Equipment // 장비
}


[CreateAssetMenu(fileName = "New Item Data", menuName = "Data/Item")] // 인스펙테어서 템 생성
public class ItemData : ScriptableObject 
{
    public ItemType itemType;
    public string itemName;
    public Sprite itemIcon;
    public string itemId;

    [Range(0,100)]
    public float dropChance;

    protected StringBuilder sb = new StringBuilder(); // 문자열 한번에

    private void OnValidate()
    {
#if UNITY_EDITOR
        string path = AssetDatabase.GetAssetPath(this);
        itemId = AssetDatabase.AssetPathToGUID(path);
#endif
    }

    public virtual string GetDescription()
    {
        return "";
    }
}
