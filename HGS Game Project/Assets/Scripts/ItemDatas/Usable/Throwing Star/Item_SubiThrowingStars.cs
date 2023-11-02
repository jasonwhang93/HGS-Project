using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_SubiThrowingStars : ItemData
{
    public Item_SubiThrowingStars() : base(
    "i0065",
    "Equipment",
    "수비 표창",
    "REQ LEV : 10 강철로 만든 표창이다. 여러 개가 들어있으며 모두 사용했다면 다시 충전해야 한다. 공격력 + 15",
    "500","250", false)
    {

    }

    public override void UseItem(object param)
    {
        base.UseItem(param);
        Debug.Log("표창을 던져서 표창을 소비합니다.");
    }
}
