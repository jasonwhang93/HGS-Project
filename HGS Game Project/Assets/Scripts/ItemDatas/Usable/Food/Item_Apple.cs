using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Apple : ItemData
{
    public Item_Apple() : base(
        "i0040", 
        "Usable", 
        "사과", 
        "빨갛게 잘 익은 사과이다. HP를 약 30 회복시킨다.", 
        "3", "1", true)
    {

    }

    public override void UseItem(object param)
    {
        base.UseItem(param);
        Debug.Log("HP가 회복됩니다.");
    }
}
