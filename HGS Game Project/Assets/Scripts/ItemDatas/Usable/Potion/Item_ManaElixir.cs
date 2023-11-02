using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_ManaElixir : ItemData
{
    public Item_ManaElixir() : base(
        "i0014",
        "Cash", 
        "마나 앨릭서", 
        "전설의 비약이다. MP를 약 300 회복시킨다.", 
        "186", "93", true)
    {
        
    }

    public override void UseItem(object param)
    {
        base.UseItem(param);
        Debug.Log("MP가 회복됩니다.");
    }
}
