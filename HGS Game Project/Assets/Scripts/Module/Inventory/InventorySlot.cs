using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySlot
{
    // 중첩될 수 있는 최대 아이템 개수
    public const int MAX_STACK_COUNT = 200;

    public ItemData data;
    public int itemCount;

    public InventorySlot(ItemData _data, int _itemCount)
    {
        data = _data;
        itemCount = _itemCount;
    }

    public void Init()
    {
        data = new Item_Null();
        itemCount = 0;
    }

    public void Insert(InventorySlot data)
    {
        if (data == null)
        {
            Debug.LogError("Attempted to insert a null InventorySlot.");
            return;
        }

        this.data = data.data;
        itemCount = data.itemCount;
    }

    // 아이템 개수를 변경하는 메서드
    public void AddItemCount(int amount)
    {
        itemCount += amount;
    }

    public void RemoveItemCount(int amount)
    {
        itemCount = Mathf.Max(itemCount - amount, 0); // 아이템의 수량이 0 미만이 되지 않도록 합니다.
    }
}
