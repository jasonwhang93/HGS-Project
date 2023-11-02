using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryContent : EventTrigger
{
    private InventoryManager inventory;
    public InventorySlot inventorySlot;
    public int slotIndex;

    [SerializeField] private Image itemSprite;
    [SerializeField] private Text itemCount;

    private bool isDragging = false;
    private int clickedSlotIndex = -1;

    private void Start()
    {
        UpdateContent();
    }

    public void SetInventoryManager(InventoryManager inventoryManger)
    {
        this.inventory = inventoryManger;
    }

    public void UpdateContent()
    {
        if (inventorySlot != null && inventorySlot.data != null)
        {
            itemSprite.sprite = inventorySlot.data.itemImage;
            itemSprite.gameObject.SetActive(inventorySlot.data.itemEnumType != ItemType.None);

            if (inventorySlot.itemCount > 1)
            {
                itemCount.text = inventorySlot.itemCount.ToString();
                itemCount.gameObject.SetActive(true);
            }
            else
            {
                itemCount.gameObject.SetActive(false);
            }
        }
        else
        {
            // 아이템이 없으면 UI를 숨김 or 초기화
            ClearItem();
        }
    }

    public bool IsEmpty()
    {
        return inventorySlot.data.itemCode == "" || inventorySlot.itemCount <= 0;
    }

    public void UpdateItem(InventorySlot slotData)
    {
        inventorySlot.Insert(slotData);
        itemSprite.sprite = slotData.data.itemImage;
        itemSprite.gameObject.SetActive(inventorySlot.data.itemEnumType != ItemType.None);

        if (slotData.data.itemIsStackable && slotData.itemCount > 1)
        {
            itemCount.text = slotData.itemCount.ToString();
            itemCount.gameObject.SetActive(true);
        }
        else
        {
            itemCount.gameObject.SetActive(false);
        }
    }

    public void ClearItem()
    {
        inventorySlot.Init();
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        // 좌클릭
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Debug.Log($"Left clicked: {slotIndex}, Name: {inventorySlot.data.itemName}, Count: {inventorySlot.itemCount}");
        }
        // 우클릭
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            Debug.Log($"Right clicked: {slotIndex}, Name: {inventorySlot.data.itemName}, Count: {inventorySlot.itemCount}");
        }
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left && inventorySlot.itemCount > 0)
        {
            isDragging = true;
            clickedSlotIndex = slotIndex;
        }
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if (isDragging)
        {
            inventory.ShowFollowCursorItem(inventorySlot.data.itemImage);
            inventory.FollowCursorItem(eventData.position);
        }
    }

    public override void OnDrop(PointerEventData eventData)
    {
        InventoryContent sourceSlot = eventData.pointerDrag.GetComponent<InventoryContent>();

        if (sourceSlot != null)
        {
            inventory.SwitchItem(sourceSlot.inventorySlot.data.itemEnumType, sourceSlot.clickedSlotIndex, this.slotIndex);
        }
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (isDragging)
        {
            isDragging = false;

            inventory.HideFollowCursorItem();
        }
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        GetComponent<Image>().color = new Color(.8f, .8f, .8f, 1f);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        GetComponent<Image>().color = Color.white;
    }
}