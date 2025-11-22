using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public enum ItemSlotContext
{
    InventorySlot,   // 인벤토리 슬롯 (일반 템칸)
    InventoryForStashSlot, // 창고 UI 안에 있는 인벤 칸 (창고로 옮기는 용도)
    EquipmentSlot,   // 장비칸 슬롯 (머리/몸/무기 등)
    StashSlot         // 창고 슬롯 (보관함)
}

public class UI_ItemSlot : MonoBehaviour , IPointerDownHandler ,IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private ItemSlotContext slotContext;

    [SerializeField] protected Image itemImage;
    [SerializeField] protected TextMeshProUGUI itemText;

    protected UI ui;
    public InventoryItem item;

    protected virtual void Start()
    {
        ui = GetComponentInParent<UI>();
    }

    public void UpdateSlot(InventoryItem _newItem) // 슬롯 업데이트
    {
        item = _newItem;

        itemImage.color = Color.white;

        if (item != null) // 아이템이 있을 경우 이미지와 텍스트 설정
        {
            itemImage.sprite = item.data.itemIcon;

            if (item.stackSize > 1)
            {
                itemText.text = item.stackSize.ToString();
            }
            else
            {
                itemText.text = "";
            }
        }
    }

    public void CleanUpSlot() // 슬롯 초기화
    {
        item = null;

        itemImage.sprite = null;
        itemImage.color = Color.clear;
        itemText.text = "";
    }

    public virtual void OnPointerDown(PointerEventData eventData) // 아이템 클릭시 동작
    {
        if (item == null || item.data == null)
            return;

        ui.itemToolTip.HideToolTip(); // 툴팁 숨기기

        // 컨텍스트에 따라 행동 분기
        switch (slotContext)
        {
            case ItemSlotContext.InventorySlot:
                HandleInventoryClick(eventData);      // 장비칸으로 보내는 쪽
                break;

            case ItemSlotContext.InventoryForStashSlot:
                HandleInventoryToStashClick(eventData);    // 창고로 보내는 쪽
                break;

            case ItemSlotContext.StashSlot:
                HandleStashClick(eventData);               // 창고 → 인벤
                break;

            case ItemSlotContext.EquipmentSlot:
                // 장비칸 클릭 시 해제 로직을 나중에 넣고 싶으면 여기에 추가
                break;
        }

    }

    public void OnPointerEnter(PointerEventData eventData) // 마우스 툴팁 표시
    {
        if (item == null)
            return;

        ui.itemToolTip.ShowToolTip(item.data as ItemData_Equipment);
    }

    public void OnPointerExit(PointerEventData eventData) // 마우스 툴팁 숨기기
    {
        if (item == null)
            return;

        ui.itemToolTip.HideToolTip();
    }

    //────────────────────────────────────────────────────────────────────
    //  인벤토리 슬롯 클릭 처리
    //────────────────────────────────────────────────────────────────────
    private void HandleInventoryClick(PointerEventData eventData)
    {
        // Ctrl + 클릭 → 아이템 삭제
        if (Input.GetKey(KeyCode.LeftControl))
        {
            Inventory.instance.RemoveItem(item.data);
            return;
        }

        // 장비라면 장착
        if (item.data.itemType == ItemType.Equipment)
        {
            Inventory.instance.EquipItem(item.data);
        }
    }

    // 창고 UI 안에 있는 인벤칸 (인벤 → 창고 이동용)
    private void HandleInventoryToStashClick(PointerEventData eventData)
    {
        // Ctrl + 클릭 → 삭제 (원하면 유지, 아니면 빼도 됨)
        if (Input.GetKey(KeyCode.LeftControl))
        {
            Inventory.instance.RemoveItem(item.data);
            return;
        }

        Inventory.instance.MoveInventoryToStash(item.data);
        return;
    }

    //────────────────────────────────────────────────────────────────────
    //  창고 슬롯 클릭 처리
    //────────────────────────────────────────────────────────────────────
    private void HandleStashClick(PointerEventData eventData)
    {
        Inventory.instance.MoveStashToInventory(item.data);
        return;
    }
}
