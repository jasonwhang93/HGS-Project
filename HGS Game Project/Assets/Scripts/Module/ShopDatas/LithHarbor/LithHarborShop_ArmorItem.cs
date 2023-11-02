using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LithHarborShop_ArmorItem : MonoBehaviour, INPC
{
    [HideInInspector] public List<string> shopItemCodeList; // 직접 초기화 제거
    public bool isSpriteDirLeft = true;

    public bool IsSpriteDirLeft { get { return isSpriteDirLeft; } }
    public List<string> ShopItemCodeList { get { return shopItemCodeList; } }

    private void Awake()
    {
        shopItemCodeList = new List<string>(); // 여기에서 초기화

        AddCode();
    }

    public void AddCode()
    {
        shopItemCodeList.Add("i0051");
        shopItemCodeList.Add("i0054");
    }

    public void PrintCode()
    {
        Debug.Log("shopItemCodeList Count = " + shopItemCodeList.Count);
    }
}
