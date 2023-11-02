using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_DexterityPotion : ItemData
{
    public Item_DexterityPotion() : base(
        "i0022", 
        "Usable", 
        "민첩함의 물약", 
        "몸놀림이 재빨라 진다.3분간 DEX가 5 증가한다", 
        "500", "250", true)
    {

    }

    public override void UseItem(object param)
    {
        base.UseItem(param);
        Debug.Log("DEX가 증가합니다.");
    }
}
