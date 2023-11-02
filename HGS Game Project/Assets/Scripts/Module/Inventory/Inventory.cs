using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    public List<InventorySlot> equipmentTypeItemList = new List<InventorySlot>();
    public List<InventorySlot> usableTypeItemList = new List<InventorySlot>();
    public List<InventorySlot> otherTypeItemList = new List<InventorySlot>();
    public List<InventorySlot> setUpTypeItemList = new List<InventorySlot>();
    public List<InventorySlot> cashTypeItemList = new List<InventorySlot>();


    public void InitializeInventory()
    {
        // 각 목록을 초기화합니다.
        InitializeListWithNull(equipmentTypeItemList);
        InitializeListWithNull(usableTypeItemList);
        InitializeListWithNull(otherTypeItemList);
        InitializeListWithNull(setUpTypeItemList);
        InitializeListWithNull(cashTypeItemList);
    }

    private void InitializeListWithNull(List<InventorySlot> itemList)
    {
        for (int i = 0; i < 24; i++)
        {
            itemList.Add(new InventorySlot(new Item_Null(), 0));
        }
    }


    public void AddItemToList(InventorySlot newSlot)
    {
        switch (newSlot.data.itemEnumType)
        {
            case ItemType.Equipment:
                if (!equipmentTypeItemList.Contains(newSlot))
                {
                    equipmentTypeItemList.Add(newSlot);
                }
                break;
            case ItemType.Usable:
                if (!usableTypeItemList.Contains(newSlot))
                {
                    usableTypeItemList.Add(newSlot);
                }
                break;
            case ItemType.Other:
                if (!otherTypeItemList.Contains(newSlot))
                {
                    otherTypeItemList.Add(newSlot);
                }
                break;
            case ItemType.SetUp:
                if (!setUpTypeItemList.Contains(newSlot))
                {
                    setUpTypeItemList.Add(newSlot);
                }
                break;
            case ItemType.Cash:
                if (!cashTypeItemList.Contains(newSlot))
                {
                    cashTypeItemList.Add(newSlot);
                }
                break;
        }
    }

    public List<InventorySlot> GetItemsByType(ItemType type)
    {
        switch (type)
        {
            case ItemType.Equipment:
                return equipmentTypeItemList;
            case ItemType.Usable:
                return usableTypeItemList;
            case ItemType.Other:
                return otherTypeItemList;
            case ItemType.SetUp:
                return setUpTypeItemList;
            case ItemType.Cash:
                return cashTypeItemList;
            default:
                Debug.LogError("Invalid ItemType provided!");
                return null;
        }
    }

    public void SwitchItem(ItemType itemType, int cell1, int cell2)
    {
        List<InventorySlot> itemList = GetItemsByType(itemType);

        if (cell1 >= 0 && cell2 >= 0)
        {
            InventorySlot temp = itemList[cell1];
            itemList[cell1] = itemList[cell2];
            itemList[cell2] = temp;
        }
        else
        {
            Debug.LogError("Invalid cell indices provided for SwitchItem function.");
        }
    }

    public void DebugPrintItemCounts()
    {
        Debug.Log($"Equipment Items: {equipmentTypeItemList.Count}");
        Debug.Log($"Usable Items: {usableTypeItemList.Count}");
        Debug.Log($"Other Items: {otherTypeItemList.Count}");
        Debug.Log($"Set Up Items: {setUpTypeItemList.Count}");
        Debug.Log($"Cash Items: {cashTypeItemList.Count}");
    }

}