using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Orange : ItemData
{
    public Item_Orange() : base(
        "i0044", 
        "Usable", 
        "오렌지", 
        "시고 달콤한 오렌지이다.MP를 약 50 회복시킨다.", 
        "10", "5", true)
    {
        
    }

    public override void UseItem(object param)
    {
        base.UseItem(param);
        Debug.Log("MP가 회복됩니다.");
    }
}
