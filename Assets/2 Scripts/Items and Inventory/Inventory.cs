using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Inventory : MonoBehaviour
{
    public static Inventory instance;

    [Header("°ÔÀÓ ½ÃÀÛ ½Ã Áö±ÞÇÒ ¾ÆÀÌÅÛ")]
    public List<ItemData> startingItems;

    [Header("Àåºñ / ÀÎº¥Åä¸® / º¸°üÇÔ µ¥ÀÌÅÍ")]
    public List<InventoryItem> equipment;
    public Dictionary<ItemData_Equipment, InventoryItem> equipmentDictionary;

    public List<InventoryItem> inventory;
    public Dictionary<ItemData, InventoryItem> inventoryDictianory;

    public List<InventoryItem> stash;
    public Dictionary<ItemData, InventoryItem> stashDictianory;

    [Header("ÀÎº¥Åä¸® UI ¿¬°á")]
    [SerializeField] private Transform inventorySlotParent;
    [SerializeField] private Transform stashSlotParent;
    [SerializeField] private Transform inventorySlotParent_Secondary; // »õ ÀÎº¥ UI
    [SerializeField] private Transform equpmentSlotParent;
    [SerializeField] private Transform statSlotParent;

    // ½½·Ô UI ¹è¿­µé
    private UI_ItemSlot[] inventoryItemSlot;
    private UI_ItemSlot[] stashItemSlot;
    private UI_ItemSlot[] inventoryItemSlot_Secondary;   // »õ ÀÎº¥ UI¿ë
    private UI_EquipmentSlot[] equipmentSlot;
    private UI_StatSlot[] statSlot;

    [Header("¾ÆÀÌÅÛ Äð´Ù¿î")]
    private float lastTimeUsedFlask;
    private float lastTimeUsedArmor;

    public float flaskCooldown { get; private set; }
    private float armorCooldown;

    [Header("¾ÆÀÌÅÛ µ¥ÀÌÅÍº£ÀÌ½º(SO)")]
    public List<ItemData> itemDataBase;


    private void Awake()
    {
        // ½Ì±ÛÅÏ
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }


    private void Start()
    {
        // ÁÖ¿ä ¸®½ºÆ®/µñ¼Å³Ê¸® ÃÊ±âÈ­
        inventory = new List<InventoryItem>();
        inventoryDictianory = new Dictionary<ItemData, InventoryItem>();

        stash = new List<InventoryItem>();
        stashDictianory = new Dictionary<ItemData, InventoryItem>();

        equipment = new List<InventoryItem>();
        equipmentDictionary = new Dictionary<ItemData_Equipment, InventoryItem>();

        // UI ½½·Ôµé Ä³½Ì
        inventoryItemSlot = inventorySlotParent.GetComponentsInChildren<UI_ItemSlot>();
        stashItemSlot = stashSlotParent.GetComponentsInChildren<UI_ItemSlot>();
        inventoryItemSlot_Secondary = inventorySlotParent_Secondary.GetComponentsInChildren<UI_ItemSlot>();
        equipmentSlot = equpmentSlotParent.GetComponentsInChildren<UI_EquipmentSlot>();
        statSlot = statSlotParent.GetComponentsInChildren<UI_StatSlot>();
    }


    public void AddStartingItems()
    {
        // ½ÃÀÛ ¾ÆÀÌÅÛ Áö±Þ
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

    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    //                 Àåºñ ±â´É
    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡

    public void EquipItem(ItemData _item)
    {
        // Àåºñ ÀåÂø Ã³¸®
        ItemData_Equipment newEquipment = _item as ItemData_Equipment;
        InventoryItem newItem = new InventoryItem(newEquipment);

        // °°Àº ºÎÀ§ Àåºñ°¡ ÀÖÀ¸¸é ±³Ã¼
        ItemData_Equipment oldEquipment = null;

        foreach (var pair in equipmentDictionary)
        {
            if (pair.Key.equipmentType == newEquipment.equipmentType)
                oldEquipment = pair.Key;
        }

        // ±âÁ¸ Àåºñ ÇØÁ¦ + ÀÎº¥Åä¸®¿¡ ³Ö±â
        if (oldEquipment != null)
        {
            UnequipItem(oldEquipment);
            AddItem(oldEquipment);
        }

        // »õ Àåºñ ÀåÂø
        equipment.Add(newItem);
        equipmentDictionary.Add(newEquipment, newItem);
        newEquipment.AddModifiers();

        // ÀÎº¥Åä¸®¿¡¼­ Á¦°Å
        RemoveItem(_item);

        UpdateSlotUI();
    }

    public void UnequipItem(ItemData_Equipment itemToRemove)
    {
        // Àåºñ ÇØÁ¦ Ã³¸®
        if (equipmentDictionary.TryGetValue(itemToRemove, out InventoryItem value))
        {
            equipment.Remove(value);
            equipmentDictionary.Remove(itemToRemove);
            itemToRemove.RemoveModifiers(); // ½ºÅÈ º¹±¸
        }
    }


    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    //                 UI ¾÷µ¥ÀÌÆ®
    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡

    public void UpdateSlotUI()
    {
        // Àåºñ ½½·Ô ÃÊ±âÈ­
        foreach (var slot in equipmentSlot)
            slot.CleanUpSlot();

        // Àåºñ ½½·Ô Ã¤¿ì±â
        foreach (var pair in equipmentDictionary)
        {
            foreach (var slot in equipmentSlot)
            {
                if (pair.Key.equipmentType == slot.slotType)
                    slot.UpdateSlot(pair.Value);
            }
        }

        // ÀÎº¥/½ºÅÂ½Ã ½½·Ô ÃÊ±âÈ­
        foreach (var s in inventoryItemSlot) s.CleanUpSlot();
        foreach (var s in stashItemSlot) s.CleanUpSlot();
        foreach (var s in inventoryItemSlot_Secondary) s.CleanUpSlot();

        // ÀÎº¥Åä¸® Àû¿ë
        for (int i = 0; i < inventory.Count; i++)
        {
            if (i < inventoryItemSlot.Length)
                inventoryItemSlot[i].UpdateSlot(inventory[i]);

            // »õ ÀÎº¥ UI¿¡µµ µ¿ÀÏÇÑ ÀÎµ¦½º·Î Ã¤¿öÁÜ
            if (inventoryItemSlot_Secondary != null && i < inventoryItemSlot_Secondary.Length)
                inventoryItemSlot_Secondary[i].UpdateSlot(inventory[i]);
        }

        // ½ºÅÂ½Ã Àû¿ë
        for (int i = 0; i < stash.Count && i < stashItemSlot.Length; i++)
            stashItemSlot[i].UpdateSlot(stash[i]);

        UpdateStatsUI();
    }

    public void UpdateStatsUI()
    {
        // ´É·ÂÄ¡ ½½·Ô UI °»½Å
        foreach (var slot in statSlot)
            slot.UpdateStatValueUI();
    }


    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    //             ¾ÆÀÌÅÛ Ãß°¡¡¤Á¦°Å
    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    public bool CanAddItem(ItemData _item)
    {
        // 1) ÀÌ¹Ì ÀÎº¥Åä¸®¿¡ ÀÖ´Â ¾ÆÀÌÅÛÀÌ¸é ½ºÅÃ¸¸ ¿Ã¸®¸é µÇ¹Ç·Î OK
        if (inventoryDictianory.ContainsKey(_item))
            return true;

        // 2) »õ ¾ÆÀÌÅÛÀÌ¸é, ºó ½½·ÔÀÌ ÀÖ´ÂÁö È®ÀÎ
        return inventory.Count < inventoryItemSlot.Length;
    }

    public void AddItem(ItemData _item)
    {
        // ÀÎº¥Åä¸® ÀÚ¸®°¡ ¾øÀ¸¸é ¸ø ¸ÔÀ½
        if (!CanAddItem(_item))
        {
            Debug.Log($"ÀÎº¥Åä¸®°¡ °¡µæ Â÷¼­ {_item.itemName} À»(¸¦) ÁÖ¿ï ¼ö ¾ø½À´Ï´Ù.");
            return;
        }

        // ÀÎº¥Åä¸®¿¡¸¸ Ãß°¡ (ÀÚµ¿À¸·Î stash·Î º¸³»Áö ¾ÊÀ½)
        AddToInventory(_item);

        UpdateSlotUI();
    }

    private void AddToStash(ItemData _item)
    {
        // ½ºÅÂ½Ã¿¡ Ãß°¡
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
        // ÀÎº¥Åä¸®¿¡ Ãß°¡
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
        // ÀÎº¥Åä¸®¿¡¼­ Á¦°Å
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

        // ½ºÅÂ½Ã¿¡¼­ Á¦°Å
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
        // ÀÎº¥Åä¸® ½½·Ô ºñ¾ú´ÂÁö
        return inventory.Count < inventoryItemSlot.Length;
    }


    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    //                 Á¦ÀÛ ±â´É
    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡

    public bool CanCraft(ItemData_Equipment craftItem, List<InventoryItem> required)
    {
        if (!CanAddItem(craftItem)) // 0) °á°ú¹°À» ³ÖÀ» ÀÎº¥Åä¸® °ø°£ÀÌ ÀÖ´ÂÁö ¸ÕÀú È®ÀÎ
        {
            Debug.Log("ÀÎº¥Åä¸®°¡ °¡µæ Â÷ ÀÖ¾î¼­ Á¦ÀÛ °á°ú¹°À» ¹ÞÀ» ¼ö ¾ø½À´Ï´Ù.");
            return false;
        }
        // Àç·á È®ÀÎ (ÀÎº¥Åä¸® + Ã¢°í ÇÕ»ê)
        foreach (var req in required)
        {
            int totalCount = 0;

            if (inventoryDictianory.TryGetValue(req.data, out InventoryItem invItem))
                totalCount += invItem.stackSize;

            if (stashDictianory.TryGetValue(req.data, out InventoryItem stashItem))
                totalCount += stashItem.stackSize;

            if (totalCount < req.stackSize)
                return false;
        }

        // Àç·á ¼Ò¸ð
        foreach (var req in required)
        {
            for (int i = 0; i < req.stackSize; i++)
                RemoveItem(req.data);
        }

        // °á°ú¹° Áö±Þ
        AddItem(craftItem);
        return true;
    }


    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    //                 Àåºñ / »ç¿ë
    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡

    public ItemData_Equipment GetEquipment(EquipmentType type)
    {
        // Æ¯Á¤ ºÎÀ§ Àåºñ ¹ÝÈ¯
        foreach (var pair in equipmentDictionary)
            if (pair.Key.equipmentType == type)
                return pair.Key;

        return null;
    }

    public void UseFlask()
    {
        // ÇÃ¶ó½ºÅ© »ç¿ë
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
        // ¹æ¾î±¸ »ç¿ë (Æ¯¼öÈ¿°ú)
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



    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    //       ¿¡µðÅÍ ±â´É (¾ÆÀÌÅÛ DB ÀÚµ¿ ¼öÁý)
    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
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

    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    //       Ã¢°í·Î ÅÛ Àü´Þ ±â´É
    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡

    public void MoveInventoryToStash(ItemData itemData)
    {
        // 1) ÀÎº¥Åä¸®¿¡ ÀÌ ¾ÆÀÌÅÛÀÌ ÀÖ´ÂÁö È®ÀÎ
        if (!inventoryDictianory.TryGetValue(itemData, out InventoryItem invItem))
            return;

        // 2) ½ºÅÃ ÇÏ³ª¸¦ Ã¢°í·Î º¸³»±â
        //    - ¸ÕÀú Ã¢°í¿¡ Ãß°¡
        AddToStash(itemData);

        // 3) ÀÎº¥Åä¸®¿¡¼­ ½ºÅÃ ÇÏ³ª »©±â
        if (invItem.stackSize <= 1)
        {
            inventory.Remove(invItem);
            inventoryDictianory.Remove(itemData);
        }
        else
        {
            invItem.RemoveStack();
        }

        UpdateSlotUI();
    }

    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    //       ÀÎº¥Åä¸®·Î ÅÛ Àü´Þ ±â´É
    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    public void MoveStashToInventory(ItemData itemData)
    {
        // 0) ÀÎº¥Åä¸®¿¡ µé¾î°¥ ÀÚ¸® ÀÖ´ÂÁö ¸ÕÀú È®ÀÎ
        if (!CanAddItem(itemData))
        {
            Debug.Log("ÀÎº¥Åä¸®°¡ °¡µæ Â÷¼­ Ã¢°í¿¡¼­ ²¨³¾ ¼ö ¾ø½À´Ï´Ù.");
            return;
        }

        // 1) Ã¢°í¿¡ ÀÌ ¾ÆÀÌÅÛÀÌ ÀÖ´ÂÁö È®ÀÎ
        if (!stashDictianory.TryGetValue(itemData, out InventoryItem stashItem))
            return;

        // 2) ÀÎº¥Åä¸®¿¡ Ãß°¡ (½ºÅÃ ÇÕÄ¡±â Æ÷ÇÔ)
        AddToInventory(itemData);

        // 3) Ã¢°í¿¡¼­ ½ºÅÃ ÇÏ³ª »©±â
        if (stashItem.stackSize <= 1)
        {
            stash.Remove(stashItem);
            stashDictianory.Remove(itemData);
        }
        else
        {
            stashItem.RemoveStack();
        }

        UpdateSlotUI();
    }

}
