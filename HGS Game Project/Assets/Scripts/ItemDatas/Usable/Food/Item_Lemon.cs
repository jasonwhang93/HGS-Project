using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Lemon : ItemData
{
    public Item_Lemon() : base(
        "i0046", 
        "Usable", 
        "레몬", 
        "매우 신 과일이다.MP를 약 150 회복시킨다.", 
        "93", "45", true)
    {

    }

    public override void UseItem(object param)
    {
        base.UseItem(param);
        Debug.Log("MP가 회복됩니다.");
    }
}
