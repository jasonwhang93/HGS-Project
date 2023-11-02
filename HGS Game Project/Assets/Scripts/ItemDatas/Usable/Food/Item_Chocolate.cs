using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Chocolate : ItemData
{
    public Item_Chocolate() : base(
        "i0082", 
        "Usable", 
        "초콜릿", 
        "달콤한 향이 진하게 나는 밀크 초콜릿이다. 초코스틱의 재료가 되기도 한다는데.. HP와 MP를 각가 1000씩 회복시켜 준다.", 
        "2100", "1500", true)
    {

    }

    public override void UseItem(object param)
    {
        base.UseItem(param);
        Debug.Log("HP와 MP가 회복됩니다.");
    }
}
