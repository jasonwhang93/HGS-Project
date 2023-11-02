using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_ReturnScrollToLithHarbor : ItemData
{
    public Item_ReturnScrollToLithHarbor() : base(
        "i0054",
        "Usable",
        "리스 항구 귀환 주문서",
        "리스 항구로 귀환할 수 있는 주문서이다. 한 번 사용하면 사라진다.",
        "500","250", true)
    {

    }

    public override void UseItem(object param)
    {
        base.UseItem(param);
        Debug.Log("리스 항구로 귀환합니다.");
    }
}
