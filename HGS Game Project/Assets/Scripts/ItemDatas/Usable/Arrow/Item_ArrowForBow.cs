using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_ArrowForBow : ItemData
{
    public Item_ArrowForBow() : base(
    "i0057",
    "SetUp",
    "활전용 화살",
    "화살이 들어있는 화살통이다. 반드시 활과 함께 사용해야 한다.",
    "1",
    "0",
    true)
    {

    }

    public override void UseItem(object param)
    {
        base.UseItem(param);
        Debug.Log("활을 쏴서 화살을 소비합니다.");
    }
}
