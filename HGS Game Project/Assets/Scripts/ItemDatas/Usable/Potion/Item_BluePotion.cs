using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_BluePotion : ItemData
{
    public Item_BluePotion() : base(
        "i0008", 
        "Usable", 
        "파란 포션", 
        "푸른 약초로 만든 물약이다. MP를 약 100 회복시킨다.", 
        "20", "10", true) 
    {

    }

    public override void UseItem(object param)
    {
        base.UseItem(param);
        Debug.Log("MP가 회복됩니다.");
    }
}
