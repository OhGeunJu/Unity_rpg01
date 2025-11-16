using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class UI_ItemSlot : MonoBehaviour , IPointerDownHandler ,IPointerEnterHandler, IPointerExitHandler
{
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
        if (item == null)
            return;

        ui.itemToolTip.HideToolTip(); // 툴팁 숨기기

        if (Input.GetKey(KeyCode.LeftControl)) // 컨트롤 키를 누른 상태에서 클릭시 아이템 삭제
        {
            Inventory.instance.RemoveItem(item.data);
            return;
        }

        if (item.data.itemType == ItemType.Equipment) // 장비 아이템 클릭시 장착
            Inventory.instance.EquipItem(item.data);

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
}
