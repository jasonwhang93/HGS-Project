using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance { get; private set; }

    public Dictionary<string, ItemData> itemDictionary;

    public List<ItemData> allEquipmentItemList;
    public List<ItemData> allUsableItemList;
    public List<ItemData> allOtherItemList;
    public List<ItemData> allSetUpItemList;
    public List<ItemData> allCashItemList;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        
        itemDictionary = new Dictionary<string, ItemData>();

        allEquipmentItemList = new List<ItemData>();
        allUsableItemList = new List<ItemData>();
        allOtherItemList = new List<ItemData>();
        allSetUpItemList = new List<ItemData>();
        allCashItemList = new List<ItemData>();

        AddUseablePotionItem();
        AddUseableFoodItem();
        AddUseableArrowItem();
        AddUseableTeleportItem();
        AddUseableThrowingStarItem();
    }

    // Start is called before the first frame update
    void Start()
    {
        //DebugItemListCount();
    }

    public void DebugItemListCount()
    {
        // 아이템 목록 디버깅 출력
        Debug.Log("Equipment Items: " + allEquipmentItemList.Count);
        Debug.Log("All Usable Items Count : " + allUsableItemList.Count);
        Debug.Log("Other Items: " + allOtherItemList.Count);
        Debug.Log("Setup Items: " + allSetUpItemList.Count);
        Debug.Log("Cash Items: " + allCashItemList.Count);
    }

    public void AddUseablePotionItem()
    {
        ItemData redPotion = new Item_RedPotion();
        AddItem(redPotion);

        ItemData orangePotion = new Item_OrangePotion();
        AddItem(orangePotion);

        ItemData whitePotion = new Item_WhitePotion();
        AddItem(whitePotion);

        ItemData bluePotion = new Item_BluePotion();
        AddItem(bluePotion);

        ItemData manaElixir = new Item_ManaElixir();
        AddItem(manaElixir);

        ItemData dexterityPotion = new Item_DexterityPotion();
        AddItem(dexterityPotion);

        ItemData speedPotion = new Item_SpeedPotion();
        AddItem(speedPotion);

        ItemData magicianPotion = new Item_MagicianPotion();
        AddItem(magicianPotion);

        ItemData warriorPotion = new Item_WarriorPotion();
        AddItem(warriorPotion);

        ItemData sniperPotion = new Item_SniperPotion();
        AddItem(sniperPotion);
    }

    public void AddUseableFoodItem()
    {
        ItemData apple = new Item_Apple();
        AddItem(apple);

        ItemData chocolate = new Item_Chocolate();
        AddItem(chocolate);

        ItemData egg = new Item_Egg();
        AddItem(egg);

        ItemData lemon = new Item_Lemon();
        AddItem(lemon);

        ItemData meat = new Item_Meat();
        AddItem(meat);

        ItemData orange = new Item_Orange();
        AddItem(orange);
    }

    public void AddUseableArrowItem()
    {
        ItemData arrowForBow = new Item_ArrowForBow();
        AddItem(arrowForBow);

        ItemData steelArrowForBow = new Item_SteelArrowForBow();
        AddItem(steelArrowForBow);
    }

    public void AddUseableTeleportItem()
    {
        ItemData returnScrollNearestTown = new Item_ReturnScrollNearestTown();
        AddItem(returnScrollNearestTown);

        ItemData returnScrollToLithHarbor = new Item_ReturnScrollToLithHarbor();
        AddItem(returnScrollToLithHarbor);
    }

    public void AddUseableThrowingStarItem()
    {
        ItemData subiThrowingStars = new Item_SubiThrowingStars();
        AddItem(subiThrowingStars);
    }

    public void AddItem(ItemData item)
    {
        // 아이템을 아이템 코드를 키로 사용하여 딕셔너리에 추가
        itemDictionary[item.itemCode] = item;

        // 아이템을 해당 아이템 타입 리스트에 추가
        switch (item.itemEnumType)
        {
            case ItemType.Equipment:
                allEquipmentItemList.Add(item);
                break;
            case ItemType.Usable:
                allUsableItemList.Add(item);
                break;
            case ItemType.Other:
                allOtherItemList.Add(item);
                break;
            case ItemType.SetUp:
                allSetUpItemList.Add(item);
                break;
            case ItemType.Cash:
                allCashItemList.Add(item);
                break;
        }
    }

    public ItemData GetItemByCode(string itemCode)
    {
        if (itemDictionary.ContainsKey(itemCode))
        {
            return itemDictionary[itemCode];
        }
        else
        {
            Debug.LogWarning("아이템 코드 " + itemCode + "에 해당하는 아이템을 찾을 수 없습니다.");
            return null;
        }
    }
}
