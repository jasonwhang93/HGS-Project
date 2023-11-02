using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Meat : ItemData
{
    public Item_Meat() : base(
        "i0050", 
        "Usable", 
        "짐승의 고기", 
        "먹음직한 짐승의 고기이다.HP를 약 100 회복시킨다.", 
        "10", "5", true)
    {

    }

    public override void UseItem(object param)
    {
        base.UseItem(param);
        Debug.Log("HP가 회복됩니다.");
    }
}
