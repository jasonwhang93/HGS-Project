using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Egg : ItemData
{
    public Item_Egg() : base(
        "i0042", 
        "Usable", 
        "달걀", 
        "영양 만점의 계란이다.HP를 약 50 회복시킨다.", 
        "5", "2", true)
    {

    }

    public override void UseItem(object param)
    {
        base.UseItem(param);
        Debug.Log("HP가 회복됩니다.");
    }
}
