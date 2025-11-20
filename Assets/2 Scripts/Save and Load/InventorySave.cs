using System.Collections.Generic;
using UnityEngine;

public class InventorySave : MonoBehaviour
{
    [Header("연결할 Inventory 본체")]
    [SerializeField] private Inventory inventory;

    private void Awake()
    {
        if (inventory == null)
            inventory = GetComponent<Inventory>();
    }

    // ------------------------------------
    //  저장
    // ------------------------------------
    public void Save()
    {
        // 일반 아이템
        Dictionary<string, int> inv = new Dictionary<string, int>();
        foreach (var pair in inventory.inventoryDictianory)
            inv[pair.Key.itemId] = pair.Value.stackSize;

        // 스태시 아이템
        foreach (var pair in inventory.stashDictianory)
            inv[pair.Key.itemId] = pair.Value.stackSize;

        // 장비
        List<string> equipped = new List<string>();
        foreach (var pair in inventory.equipmentDictionary)
            equipped.Add(pair.Key.itemId);

        ES3.Save(SaveKeys.Inventory, inv);
        ES3.Save(SaveKeys.EquipmentIds, equipped);
    }

    // ------------------------------------
    //  로드
    // ------------------------------------
    public void Load()
    {
        // 기본값
        Dictionary<string, int> inv =
            ES3.Load<Dictionary<string, int>>(SaveKeys.Inventory, new Dictionary<string, int>());

        List<string> equipped =
            ES3.Load<List<string>>(SaveKeys.EquipmentIds, new List<string>());

        // 현재 모든 데이터 리셋
        inventory.inventory.Clear();
        inventory.inventoryDictianory.Clear();

        inventory.stash.Clear();
        inventory.stashDictianory.Clear();

        inventory.equipment.Clear();
        inventory.equipmentDictionary.Clear();

        // 로드된 일반/스태시 아이템 적용
        foreach (var pair in inv)
        {
            string itemId = pair.Key;
            int count = pair.Value;

            ItemData item = inventory.itemDataBase.Find(x => x.itemId == itemId);
            if (item == null)
                continue;

            for (int i = 0; i < count; i++)
                inventory.AddItem(item);
        }

        // 로드된 장비 적용
        foreach (string id in equipped)
        {
            ItemData eq = inventory.itemDataBase.Find(x => x.itemId == id);
            if (eq != null)
                inventory.EquipItem(eq);
        }

        inventory.UpdateSlotUI();
    }

    // ------------------------------------
    //  새 게임 초기화
    // ------------------------------------
    public void ResetToDefault()
    {
        inventory.inventory.Clear();
        inventory.inventoryDictianory.Clear();
        inventory.stash.Clear();
        inventory.stashDictianory.Clear();
        inventory.equipment.Clear();
        inventory.equipmentDictionary.Clear();

        ES3.DeleteKey(SaveKeys.Inventory);
        ES3.DeleteKey(SaveKeys.EquipmentIds);

        inventory.UpdateSlotUI();
    }
}
