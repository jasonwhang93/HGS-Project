using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShopContent : MonoBehaviour
{
    // 아이템 이미지, 이름, 판매가격에 대한 public 변수
    public Image itemImage;
    public Text itemNameText;
    public Text itemPriceText;
    public Image itemCointImage;
    public Text itemCountText;

    // 아이템 데이터
    private ItemData itemData;

    public Image cellSprite1;
    public Image cellSprite2;

    // 현재 선택된 상점 아이템에 대한 참조
    [HideInInspector] public static ShopContent currentSelectedShopContent = null;
    private Color defaultColor = Color.white;
    private Color disabledColor = new Color(0, 240 / 255f, 255 / 255f, 1);

    private float lastClickTime = 0f;
    private const float DOUBLE_CLICK_DELAY = 0.25f;

    [HideInInspector] public bool isSellingItem = false;

    public ItemData ItemData
    {
        get { return itemData; }
    }

    // 버튼 클릭 시 호출될 메서드
    public void OnButtonClicked()
    {
        // 이전에 선택된 상점 아이템이 있고 현재 상점 아이템과 다른 경우
        if (currentSelectedShopContent != null && currentSelectedShopContent != this)
        {
            currentSelectedShopContent.cellSprite1.color = defaultColor;
            currentSelectedShopContent.cellSprite2.color = defaultColor;
        }

        // 현재 상점 아이템을 선택된 상점 아이템으로 설정
        currentSelectedShopContent = this;

        cellSprite1.color = disabledColor;
        cellSprite2.color = disabledColor;

        if ((Time.time - lastClickTime) < DOUBLE_CLICK_DELAY)
        {
            EventManager.instance.SendEvent("Shop CountNoticeBox :: UIToggle");

            string eventName = "";
            if (isSellingItem) // 판매인 경우
            {
                eventName = itemData.itemIsStackable ? "OpenSellCountNoticeBox" : "OpenSellNoticeBox";

            }
            else // 구매인 경우
            {
                eventName = itemData.itemIsStackable ? "OpenBuyCountNoticeBox" : "OpenBuyNoticeBox";
            }
            EventManager.instance.SendEvent(eventName);
        }
        lastClickTime = Time.time;
    }

    // 아이템 정보 설정 메서드
    public void SetItemData(ItemData data, bool isSelling)
    {
        itemData = data;
        isSellingItem = isSelling;

        bool isItemNull = itemData is Item_Null;  // 현재 ItemData 객체가 Item_Null 타입인지 확인합니다.

        // 이미지 설정
        if (itemImage != null)
        {
            itemImage.sprite = itemData.itemImage;
            itemImage.enabled = !isItemNull;
        }

        // 아이템 이름 설정
        if (itemNameText != null)
        {
            itemNameText.text = itemData.itemName;
            itemNameText.enabled = !isItemNull; // Item_Null이면 이름 텍스트를 숨깁니다.
        }

        // 아이템 구매 가격 설정
        if (itemPriceText != null && !isSelling)
        {
            itemPriceText.text = itemData.itemPurchasePrice.ToString();
            itemPriceText.enabled = !isItemNull; // Item_Null이면 가격 텍스트를 숨깁니다.
        }

        if(itemPriceText != null && isSelling)
        {
            itemPriceText.text = itemData.itemSellPrice.ToString();
            itemPriceText.enabled = !isItemNull;
        }

        // 상점 컨텐츠 코인 아이콘 이미지 설정
        if (itemCointImage != null)
        {
            itemCointImage.enabled = !isItemNull;
        }
    }

    // 아이템 수량 설정 메서드
    // 아이템 수량 설정 메서드
    public void SetItemCount(InventorySlot slot, bool isSelling)
    {
        if (itemCountText != null && slot != null)
        {
            if (slot.data.itemIsStackable && !isSelling) // isStackable은 중첩 가능한 아이템인지 확인하는 속성이라고 가정합니다.
            {
                itemCountText.text = slot.itemCount.ToString();
                itemCountText.enabled = true;
            }
            else
            {
                itemCountText.text = "";
                itemCountText.enabled = false;
            }
        }
    }

    public void ClearItem()
    {
        // 아이템 정보 및 이미지 등을 초기화
        itemData = null;
        if (itemImage != null) itemImage.sprite = null;
        if (itemNameText != null) itemNameText.text = "";
        if (itemPriceText != null) itemPriceText.text = "";
    }

}
