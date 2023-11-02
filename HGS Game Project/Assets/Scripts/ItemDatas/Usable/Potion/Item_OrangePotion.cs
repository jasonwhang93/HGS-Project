using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_OrangePotion : ItemData
{
    public Item_OrangePotion() : base(
        "i0004", 
        "Usable", 
        "주황 포션", 
        "붉은 약초의 농축 물약이다. HP를 약 150 회복시킨다.", 
        "48", "24", true)
    {

    }

    public override void UseItem(object param)
    {
        base.UseItem(param);
        Debug.Log("HP가 회복됩니다.");
    }
}
