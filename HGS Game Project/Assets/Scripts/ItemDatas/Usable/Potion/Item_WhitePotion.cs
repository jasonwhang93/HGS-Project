using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_WhitePotion : ItemData
{
    public Item_WhitePotion() : base(
        "i0006", 
        "Usable", 
        "하얀 포션", 
        "붉은 약초의 고농축 물약이다. HP를 약 300 회복시킨다.", 
        "96", "48", true)
    {

    }

    public override void UseItem(object param)
    {
        base.UseItem(param);
        Debug.Log("HP가 회복됩니다.");
    }
}
