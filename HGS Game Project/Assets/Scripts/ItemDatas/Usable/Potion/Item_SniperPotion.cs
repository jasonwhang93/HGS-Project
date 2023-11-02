using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_SniperPotion : ItemData
{
    public Item_SniperPotion() : base(
        "i0032", 
        "Usable", 
        "명사수의 물약", 
        "민첩성을 증가시켜 준다. 5분간 DEX가 5 증가한다.", 
        "500", "250", true)
    {

    }

    public override void UseItem(object param)
    {
        base.UseItem(param);
        Debug.Log("DEX가 증가합니다.");
    }
}
