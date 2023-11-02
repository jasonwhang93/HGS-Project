using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_MagicianPotion : ItemData
{
    public Item_MagicianPotion() : base(
        "i0026", 
        "Usable", 
        "마법사의 물약", 
        "마력을 향상시켜 준다. 3분간 마력이 5 증가한다.", 
        "500", "250", true)
    {

    }

    public override void UseItem(object param)
    {
        base.UseItem(param);
        Debug.Log("마력이 증가합니다.");
    }
}
