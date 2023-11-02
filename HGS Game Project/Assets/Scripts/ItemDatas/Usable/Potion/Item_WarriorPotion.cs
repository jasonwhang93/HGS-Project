using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_WarriorPotion : ItemData
{
    public Item_WarriorPotion() : base(
        "i0030", 
        "Usable", 
        "전사의 물약", 
        "공격력을 증가시켜 준다. 3분간 공격력이 5 증가한다.", 
        "500", "250", true)
    {

    }

    public override void UseItem(object param)
    {
        base.UseItem(param);
        Debug.Log("공격력이 증가합니다.");
    }
}
