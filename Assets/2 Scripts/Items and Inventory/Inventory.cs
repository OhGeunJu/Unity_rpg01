using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Inventory : MonoBehaviour
{
    public static Inventory instance;

    [Header("게임 시작 시 지급할 아이템")]
    public List<ItemData> startingItems;

    [Header("장비 / 인벤토리 / 보관함 데이터")]
    public List<InventoryItem> equipment;
    public Dictionary<ItemData_Equipment, InventoryItem> equipmentDictionary;

    public List<InventoryItem> inventory;
    public Dictionary<ItemData, InventoryItem> inventoryDictianory;

    public List<InventoryItem> stash;
    public Dictionary<ItemData, InventoryItem> stashDictianory;

    [Header("인벤토리 UI 연결")]
    [SerializeField] private Transform inventorySlotParent;
    [SerializeField] private Transform stashSlotParent;
    [SerializeField] private Transform equpmentSlotParent;
    [SerializeField] private Transform statSlotParent;

    // 슬롯 UI 배열들
    private UI_ItemSlot[] inventoryItemSlot;
    private UI_ItemSlot[] stashItemSlot;
    private UI_EquipmentSlot[] equipmentSlot;
    private UI_StatSlot[] statSlot;

    [Header("아이템 쿨다운")]
    private float lastTimeUsedFlask;
    private float lastTimeUsedArmor;

    public float flaskCooldown { get; private set; }
    private float armorCooldown;

    [Header("아이템 데이터베이스(SO)")]
    public List<ItemData> itemDataBase;


    private void Awake()
    {
        // 싱글턴
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }


    private void Start()
    {
        // 주요 리스트/딕셔너리 초기화
        inventory = new List<InventoryItem>();
        inventoryDictianory = new Dictionary<ItemData, InventoryItem>();

        stash = new List<InventoryItem>();
        stashDictianory = new Dictionary<ItemData, InventoryItem>();

        equipment = new List<InventoryItem>();
        equipmentDictionary = new Dictionary<ItemData_Equipment, InventoryItem>();

        // UI 슬롯들 캐싱
        inventoryItemSlot = inventorySlotParent.GetComponentsInChildren<UI_ItemSlot>();
        stashItemSlot = stashSlotParent.GetComponentsInChildren<UI_ItemSlot>();
        equipmentSlot = equpmentSlotParent.GetComponentsInChildren<UI_EquipmentSlot>();
        statSlot = statSlotParent.GetComponentsInChildren<UI_StatSlot>();

        AddStartingItems(); // 시작 아이템 지급
    }


    private void AddStartingItems()
    {
        // 시작 아이템 지급
        foreach (var item in startingItems)
        {
            if (item != null)
                AddItem(item);
        }
    }

    public List<InventoryItem> GetEquipmentList()
    {
        return equipment;
    }

    public List<InventoryItem> GetStashList()
    {
        return stash;
    }

    // ─────────────────────────────────────────────
    //                 장비 기능
    // ─────────────────────────────────────────────

    public void EquipItem(ItemData _item)
    {
        // 장비 장착 처리
        ItemData_Equipment newEquipment = _item as ItemData_Equipment;
        InventoryItem newItem = new InventoryItem(newEquipment);

        // 같은 부위 장비가 있으면 교체
        ItemData_Equipment oldEquipment = null;

        foreach (var pair in equipmentDictionary)
        {
            if (pair.Key.equipmentType == newEquipment.equipmentType)
                oldEquipment = pair.Key;
        }

        // 기존 장비 해제 + 인벤토리에 넣기
        if (oldEquipment != null)
        {
            UnequipItem(oldEquipment);
            AddItem(oldEquipment);
        }

        // 새 장비 장착
        equipment.Add(newItem);
        equipmentDictionary.Add(newEquipment, newItem);
        newEquipment.AddModifiers();

        // 인벤토리에서 제거
        RemoveItem(_item);

        UpdateSlotUI();
    }

    public void UnequipItem(ItemData_Equipment itemToRemove)
    {
        // 장비 해제 처리
        if (equipmentDictionary.TryGetValue(itemToRemove, out InventoryItem value))
        {
            equipment.Remove(value);
            equipmentDictionary.Remove(itemToRemove);
            itemToRemove.RemoveModifiers(); // 스탯 복구
        }
    }


    // ─────────────────────────────────────────────
    //                 UI 업데이트
    // ─────────────────────────────────────────────

    public void UpdateSlotUI()
    {
        // 장비 슬롯 초기화
        foreach (var slot in equipmentSlot)
            slot.CleanUpSlot();

        // 장비 슬롯 채우기
        foreach (var pair in equipmentDictionary)
        {
            foreach (var slot in equipmentSlot)
            {
                if (pair.Key.equipmentType == slot.slotType)
                    slot.UpdateSlot(pair.Value);
            }
        }

        // 인벤/스태시 슬롯 초기화
        foreach (var s in inventoryItemSlot) s.CleanUpSlot();
        foreach (var s in stashItemSlot) s.CleanUpSlot();

        // 인벤토리 적용
        for (int i = 0; i < inventory.Count && i < inventoryItemSlot.Length; i++)
            inventoryItemSlot[i].UpdateSlot(inventory[i]);

        // 스태시 적용
        for (int i = 0; i < stash.Count && i < stashItemSlot.Length; i++)
            stashItemSlot[i].UpdateSlot(stash[i]);

        UpdateStatsUI();
    }

    public void UpdateStatsUI()
    {
        // 능력치 슬롯 UI 갱신
        foreach (var slot in statSlot)
            slot.UpdateStatValueUI();
    }


    // ─────────────────────────────────────────────
    //             아이템 추가·제거
    // ─────────────────────────────────────────────

    public void AddItem(ItemData _item)
    {
        // 장비/재료 구분하여 넣기
        if (_item.itemType == ItemType.Equipment && CanAddItem())
            AddToInventory(_item);
        else if (_item.itemType == ItemType.Material)
            AddToStash(_item);

        UpdateSlotUI();
    }

    private void AddToStash(ItemData _item)
    {
        // 스태시에 추가
        if (stashDictianory.TryGetValue(_item, out InventoryItem value))
            value.AddStack();
        else
        {
            InventoryItem newItem = new InventoryItem(_item);
            stash.Add(newItem);
            stashDictianory.Add(_item, newItem);
        }
    }

    private void AddToInventory(ItemData _item)
    {
        // 인벤토리에 추가
        if (inventoryDictianory.TryGetValue(_item, out InventoryItem value))
            value.AddStack();
        else
        {
            InventoryItem newItem = new InventoryItem(_item);
            inventory.Add(newItem);
            inventoryDictianory.Add(_item, newItem);
        }
    }

    public void RemoveItem(ItemData _item)
    {
        // 인벤토리에서 제거
        if (inventoryDictianory.TryGetValue(_item, out InventoryItem invValue))
        {
            if (invValue.stackSize <= 1)
            {
                inventory.Remove(invValue);
                inventoryDictianory.Remove(_item);
            }
            else
                invValue.RemoveStack();
        }

        // 스태시에서 제거
        if (stashDictianory.TryGetValue(_item, out InventoryItem stashValue))
        {
            if (stashValue.stackSize <= 1)
            {
                stash.Remove(stashValue);
                stashDictianory.Remove(_item);
            }
            else
                stashValue.RemoveStack();
        }

        UpdateSlotUI();
    }

    public bool CanAddItem()
    {
        // 인벤토리 슬롯 비었는지
        return inventory.Count < inventoryItemSlot.Length;
    }


    // ─────────────────────────────────────────────
    //                 제작 기능
    // ─────────────────────────────────────────────

    public bool CanCraft(ItemData_Equipment craftItem, List<InventoryItem> required)
    {
        // 재료 확인
        foreach (var req in required)
        {
            if (!stashDictianory.TryGetValue(req.data, out InventoryItem stashItem)
                || stashItem.stackSize < req.stackSize)
                return false;
        }

        // 재료 소모
        foreach (var req in required)
        {
            for (int i = 0; i < req.stackSize; i++)
                RemoveItem(req.data);
        }

        // 결과물 지급
        AddItem(craftItem);
        return true;
    }


    // ─────────────────────────────────────────────
    //                 장비 / 사용
    // ─────────────────────────────────────────────

    public ItemData_Equipment GetEquipment(EquipmentType type)
    {
        // 특정 부위 장비 반환
        foreach (var pair in equipmentDictionary)
            if (pair.Key.equipmentType == type)
                return pair.Key;

        return null;
    }

    public void UseFlask()
    {
        // 플라스크 사용
        var flask = GetEquipment(EquipmentType.Flask);
        if (flask == null)
            return;

        if (Time.time > lastTimeUsedFlask + flaskCooldown)
        {
            flaskCooldown = flask.itemCooldown;
            flask.Effect(null);
            lastTimeUsedFlask = Time.time;
        }
    }

    public bool CanUseArmor()
    {
        // 방어구 사용 (특수효과)
        var armor = GetEquipment(EquipmentType.Armor);
        if (armor == null)
            return false;

        if (Time.time > lastTimeUsedArmor + armorCooldown)
        {
            armorCooldown = armor.itemCooldown;
            lastTimeUsedArmor = Time.time;
            return true;
        }

        return false;
    }



    // ─────────────────────────────────────────────
    //       에디터 기능 (아이템 DB 자동 수집)
    // ─────────────────────────────────────────────
#if UNITY_EDITOR
    [ContextMenu("Fill up item data base")]
    private void FillUpItemDataBase() => itemDataBase = new List<ItemData>(GetItemDataBase());

    private List<ItemData> GetItemDataBase()
    {
        List<ItemData> list = new List<ItemData>();
        string[] assets = AssetDatabase.FindAssets("", new[] { "Assets/Data/Items" });

        foreach (string guid in assets)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var data = AssetDatabase.LoadAssetAtPath<ItemData>(path);
            list.Add(data);
        }

        return list;
    }
#endif
}
