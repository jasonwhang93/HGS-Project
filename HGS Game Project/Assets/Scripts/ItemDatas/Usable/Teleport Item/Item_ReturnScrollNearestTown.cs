using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_ReturnScrollNearestTown : ItemData
{
    public Item_ReturnScrollNearestTown() : base(
        "i0051",
        "Other", 
        "마을 귀환 주문서", 
        "현재 위치에서 가장 가까운 마을로 귀환할 수 있는 주문서이다. 한 번 사용하면 사라진다. 모든 마을 잡화상점에서 구입할 수 있다.", 
        "400", "200", true)
    {

    }

    public override void UseItem(object param)
    {
        base.UseItem(param);
        Debug.Log("가장 가까운 마을로 귀환합니다.");
    }
}
