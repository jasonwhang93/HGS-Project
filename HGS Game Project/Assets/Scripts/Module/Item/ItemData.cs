using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemData : ItemBase
{
    public ItemData(string code, string type, string name, string descript, string purchasePrice, string sellPrice, bool isStackable)
    {
        itemCode = code;
        itemStringType = type;
        itemName = name;
        itemDescription = descript;
        itemPurchasePrice = purchasePrice;
        itemSellPrice = sellPrice;
        itemIsStackable = isStackable;

        itemImage = Resources.Load<Sprite>($"Sprite/Item/Useable/{code}");
        itemEnumType = SetItemType(type);
    }

    public override void UseItem(object param)
    {
        // 아이템 사용에 필요한 동작을 구현
        Debug.Log($"{itemName}을(를) 사용합니다.");
    }
}
