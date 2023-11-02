using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Null : ItemData
{
    public Item_Null() : base("", "None", "비어있는 아이템", "", "-1", "-1", false)
    {
        // 비어있는 아이템임
    }

    public override void UseItem(object param)
    {
        // 비어있는 아이템이기 때문에 아무 동작도 하지 않음
    }
}
