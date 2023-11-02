using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_SpeedPotion : ItemData
{
    public Item_SpeedPotion() : base(
        "i0024", 
        "Usable", 
        "속도향상의 물약", 
        "빠르게 이동할 수 있다.3분간 이동속도가 증가한다.", 
        "400", "200", true)
    {

    }

    public override void UseItem(object param)
    {
        base.UseItem(param);
        Debug.Log("이동속도가 증가합니다.");
    }
}
