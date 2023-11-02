using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_RedPotion : ItemData
{
    public Item_RedPotion() : base(
        "i0002", 
        "Usable", 
        "빨간 포션", 
        "붉은 약초로 만든 물약이다. HP를 약 50 회복시킨다.", 
        "5", "3", true) 
    {

    }

    public override void UseItem(object param)
    {
        base.UseItem(param);
        Debug.Log("HP가 회복됩니다.");
    }
}
