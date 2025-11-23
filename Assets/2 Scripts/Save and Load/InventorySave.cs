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

        SaveManager.Instance.RegisterInventory(this);
    }

    // ------------------------------------
    //  저장
    // ------------------------------------
    public void Save()
    {
        // 1) 인벤토리 아이템
        Dictionary<string, int> inv = new Dictionary<string, int>();
        foreach (var pair in inventory.inventoryDictianory)
            inv[pair.Key.itemId] = pair.Value.stackSize;

        // 2) 스태시 아이템
        Dictionary<string, int> stash = new Dictionary<string, int>();
        foreach (var pair in inventory.stashDictianory)
            stash[pair.Key.itemId] = pair.Value.stackSize;

        // 3) 장비
        List<string> equipped = new List<string>();
        foreach (var pair in inventory.equipmentDictionary)
            equipped.Add(pair.Key.itemId);

        ES3.Save(SaveKeys.Inventory, inv);
        ES3.Save(SaveKeys.Stash, stash);          // ★ 새 키 필요
        ES3.Save(SaveKeys.EquipmentIds, equipped);
    }

    // ------------------------------------
    //  로드
    // ------------------------------------
    public void Load()
    {
        // 저장된 데이터 불러오기
        Dictionary<string, int> inv =
            ES3.Load<Dictionary<string, int>>(SaveKeys.Inventory, new Dictionary<string, int>());

        Dictionary<string, int> stash =
        ES3.Load<Dictionary<string, int>>(SaveKeys.Stash, new Dictionary<string, int>());

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

            // AddToInventory 로직을 그대로 여기서 재현
            if (inventory.inventoryDictianory.TryGetValue(item, out InventoryItem invItem))
            {
                invItem.stackSize += count;
            }
            else
            {
                InventoryItem newItem = new InventoryItem(item);
                newItem.stackSize = count; // 생성자에서 1 올리지만 어차피 여기서 덮어씀
                inventory.inventory.Add(newItem);
                inventory.inventoryDictianory.Add(item, newItem);
            }
        }

        foreach (var pair in stash)
        {
            string itemId = pair.Key;
            int count = pair.Value;

            ItemData item = inventory.itemDataBase.Find(x => x.itemId == itemId);
            if (item == null)
                continue;

            if (inventory.stashDictianory.TryGetValue(item, out InventoryItem stashItem))
            {
                stashItem.stackSize += count;
            }
            else
            {
                InventoryItem newItem = new InventoryItem(item);
                newItem.stackSize = count;
                inventory.stash.Add(newItem);
                inventory.stashDictianory.Add(item, newItem);
            }
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
        ES3.DeleteKey(SaveKeys.Stash);
        ES3.DeleteKey(SaveKeys.EquipmentIds);

        inventory.AddStartingItems();

        inventory.UpdateSlotUI();
    }
}
