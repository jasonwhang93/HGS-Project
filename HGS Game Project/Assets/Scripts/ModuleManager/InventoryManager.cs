using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public Inventory inventory; // 인벤토리 객체

    [SerializeField] private GameObject inventoryUI; // 인벤토리 UI GameObject

    [SerializeField] private Button[] tabButton; // 탭 버튼 배열
    private int tabButtonIndex = 1; // 현재 선택된 탭 버튼 인덱스

    [SerializeField] private GameObject inventoryContentPrefab; // 인벤토리 슬롯을 생성할 프리팹
    [SerializeField] private Transform parentForContents; // 슬롯을 생성할 부모 Transform

    private List<InventoryContent> inventorySlots = new List<InventoryContent>(); // 인벤토리 슬롯을 관리하는 리스트

    [SerializeField] private int inventorySlotCount = 24; // 인벤토리 슬롯의 개수

    [SerializeField] private Transform followCursor; // 아이템 따라다니는 커서 Transform

    [SerializeField] private Text playerMoneyText; // 플레이어 소지금을 표시하는 UI Text

    private void Awake()
    {
        inventory = new Inventory(); // 인벤토리 초기화

        CreateInventorySlots(); // 인벤토리 슬롯 생성
    }

    private void Start()
    {
        AddItemFunc(); // 아이템 초기 추가

        DisplayUseableItems(); // 사용 가능한 아이템 탭 표시

        // 인벤토리 UI 토글 이벤트 리스너 등록
        EventManager.instance.AddEvent("Inventory :: UIToggle", (p) =>
        {
            // 인벤토리 창의 활성화 상태를 토글
            inventoryUI.SetActive(!inventoryUI.activeInHierarchy);
            UpdateInventoryUI((ItemType)(tabButtonIndex + 1));
            UpdatePlayerMoneyUI(); // 플레이어 소지금 업데이트
        });
    }

    private void Update()
    {
        if (inventoryUI.activeInHierarchy)
        {
            tabButton[tabButtonIndex].Select(); // 활성화된 인벤토리 UI에서 현재 선택된 탭 버튼을 선택 상태로 설정
        }
    }

    private void CreateInventorySlots()
    {
        if (inventoryContentPrefab == null || parentForContents == null)
        {
            Debug.LogError("Inventory Content Prefab or Parent Transform is not set in the inspector!");
            return;
        }

        // 인벤토리 슬롯을 생성하고 리스트에 추가
        for (int i = 0; i < inventorySlotCount; i++)
        {
            GameObject createdSlot = Instantiate(inventoryContentPrefab, parentForContents);
            InventoryContent contentComponent = createdSlot.GetComponent<InventoryContent>();

            if (contentComponent != null)
            {
                contentComponent.slotIndex = i;
                contentComponent.inventorySlot = new InventorySlot(new Item_Null(), 0); // InventorySlot 객체 생성
                contentComponent.SetInventoryManager(this);
                inventorySlots.Add(contentComponent);  // 슬롯 리스트에 추가
            }
        }

        inventory.InitializeInventory(); // 인벤토리 초기화
    }

    private void AddItemFunc()
    {
        // 장비아이템 추가

        // 소비아이템 추가
        AddItem(new Item_RedPotion(), 3);
        AddItem(new Item_BluePotion(), 3);
        AddItem(new Item_OrangePotion(), 3);
        AddItem(new Item_WhitePotion(), 3);

        // 기타아이템 추가

        // 설치아이템 추가

        // 캐시아이템 추가
    }

    // 아이템을 인벤토리에 추가하는 함수
    public void AddItem(ItemData data, int count = 1)
    {
        List<InventorySlot> currentItemList = inventory.GetItemsByType(data.itemEnumType);

        if (count > 0) // 아이템 추가 로직
        {
            if (data.itemIsStackable) // 아이템이 중첩 가능한 경우
            {
                foreach (var slot in currentItemList)
                {
                    if (slot.data.itemCode == data.itemCode)
                    {
                        int spaceInSlot = InventorySlot.MAX_STACK_COUNT - slot.itemCount;

                        if (spaceInSlot >= count)
                        {
                            slot.AddItemCount(count);
                            count = 0;
                            break;
                        }
                        else
                        {
                            slot.AddItemCount(spaceInSlot);
                            count -= spaceInSlot;
                        }
                    }
                }
            }

            // 아직 추가되지 않은 아이템이 남아있는 경우, 새로운 빈 슬롯에 추가합니다.
            while (count > 0)
            {
                int amountToAdd = data.itemIsStackable ? System.Math.Min(count, InventorySlot.MAX_STACK_COUNT) : 1;
                InventorySlot newSlot = new InventorySlot(data, amountToAdd);

                bool slotFound = false;

                // Item_Null로 초기화된 슬롯을 찾아서 아이템 추가
                for (int i = 0; i < currentItemList.Count; i++)
                {
                    if (currentItemList[i].data is Item_Null)
                    {
                        currentItemList[i] = newSlot;
                        slotFound = true;
                        count -= amountToAdd;
                        break;
                    }
                }

                // 더 이상 빈 슬롯이 없으면 중단합니다.
                if (!slotFound)
                {
                    Debug.LogWarning("No available slot to add item!");
                    break;
                }
            }
        }
        else if (count < 0) // 아이템 감소 로직
        {
            int totalItemCount = 0;

            foreach (var slot in currentItemList)
            {
                if (slot.data.itemCode == data.itemCode)
                {
                    totalItemCount += slot.itemCount;
                }
            }

            if (totalItemCount + count >= 0) // 총 아이템 수가 감소시키고자 하는 수량보다 큰지 확인
            {
                int amountToRemove = -count; // 양수로 변환

                foreach (var slot in currentItemList)
                {
                    if (slot.data.itemCode == data.itemCode && !(slot.itemCount == InventorySlot.MAX_STACK_COUNT))
                    {
                        if (slot.itemCount <= amountToRemove)
                        {
                            amountToRemove -= slot.itemCount;
                            slot.Init(); // 아이템 슬롯 초기화
                        }
                        else
                        {
                            slot.RemoveItemCount(amountToRemove);
                            amountToRemove = 0;
                            break;
                        }
                    }
                }

                // 남은 수량이 있다면, 꽉 차있는 슬롯에서 감소시킨다.
                if (amountToRemove > 0)
                {
                    foreach (var slot in currentItemList)
                    {
                        if (slot.data.itemCode == data.itemCode)
                        {
                            if (slot.itemCount > amountToRemove)
                            {
                                slot.RemoveItemCount(amountToRemove);
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                Debug.LogWarning("Attempting to remove more items than available!");
                return;  // 여기에서 return을 사용하여 메서드를 종료합니다.
            }
        }

        // 인벤토리 UI 업데이트
        UpdateInventoryUI(data.itemEnumType);
    }


    // 아이템 유형에 따라 인벤토리 UI를 업데이트하는 함수
    public void UpdateInventoryUI(ItemType itemType)
    {
        // 아이템 유형별로 UI를 갱신
        var items = inventory.GetItemsByType(itemType);

        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (i < items.Count)
            {
                inventorySlots[i].UpdateItem(items[i]);
            }
            else
            {
                inventorySlots[i].ClearItem();
            }
        }
    }

    // 장비 아이템 탭 표시
    public void DisplayEquipmentItems()
    {
        ClearAllSlots(); // 모든 슬롯을 초기화

        tabButtonIndex = 0;

        UpdateInventoryUI(ItemType.Equipment);
    }

    // 소비 아이템 탭 표시
    public void DisplayUseableItems()
    {
        ClearAllSlots(); // 모든 슬롯을 초기화

        tabButtonIndex = 1;

        UpdateInventoryUI(ItemType.Usable);
    }

    // 기타 아이템 탭 표시
    public void DisplayOtherItems()
    {
        ClearAllSlots(); // 모든 슬롯을 초기화

        tabButtonIndex = 2;

        UpdateInventoryUI(ItemType.Other);
    }

    // 설치 아이템 탭 표시
    public void DisplaySetUpItems()
    {
        ClearAllSlots(); // 모든 슬롯을 초기화

        tabButtonIndex = 3;

        UpdateInventoryUI(ItemType.SetUp);
    }

    // 캐시 아이템 탭 표시
    public void DisplayCashItems()
    {
        ClearAllSlots(); // 모든 슬롯을 초기화

        tabButtonIndex = 4;

        UpdateInventoryUI(ItemType.Cash);
    }

    // 모든 슬롯을 초기화하는 함수
    private void ClearAllSlots()
    {
        foreach (InventoryContent slot in inventorySlots)
        {
            slot.ClearItem();
        }
    }

    // 아이템을 스왑하는 함수
    public void SwitchItem(ItemType itemType, int cell1, int cell2)
    {
        if (cell1 < 0 || cell2 < 0) return;

        inventory.SwitchItem(itemType, cell1, cell2);

        UpdateInventoryUI(itemType);
    }

    // 마우스 커서 위에 아이템 이미지를 표시하는 함수
    public void ShowFollowCursorItem(Sprite sprite)
    {
        followCursor.GetComponent<Image>().sprite = sprite;
        followCursor.gameObject.SetActive(true);
    }

    // 마우스 커서 위에 표시된 아이템 이미지를 숨기는 함수
    public void HideFollowCursorItem()
    {
        followCursor.gameObject.SetActive(false);
    }

    // 마우스 커서 위에 표시된 아이템 이미지를 따라가게 하는 함수
    public void FollowCursorItem(Vector2 mousePosition)
    {
        followCursor.position = mousePosition;
    }

    // 플레이어 소지금 정보를 업데이트하는 함수
    public void UpdatePlayerMoneyUI()
    {
        //playerMoneyText.text = playerData.playerMoney.ToString();
        playerMoneyText.text = string.Format("{0:N0}", PlayerData.playerMoney);
    }

    // 아이템을 아이템 가치(itemPurchasePrice)를 기준으로 정렬하는 함수
    public void SortInventoryByValue()
    {
        // 아이템 목록을 가져옵니다. 아이템 유형에 따라 가져올 수 있도록 구현되어 있어야 합니다.
        List<InventorySlot> itemList = inventory.GetItemsByType((ItemType)(tabButtonIndex + 1));

        itemList.Sort((slot1, slot2) =>
        {
            string value1Str = slot1.data.itemPurchasePrice;
            string value2Str = slot2.data.itemPurchasePrice;

            if (value1Str == "-1" && value2Str != "-1")
            {
                return 1;
            }
            else if (value1Str != "-1" && value2Str == "-1")
            {
                return -1;
            }
            else if (value1Str == "-1" && value2Str == "-1")
            {
                return 0;
            }
            else
            {
                int value1 = int.Parse(value1Str);
                int value2 = int.Parse(value2Str);
                return value1.CompareTo(value2);
            }
        });

        // 정렬된 아이템 목록을 다시 인벤토리에 추가합니다.
        foreach (InventorySlot slot in itemList)
        {
            inventory.AddItemToList(slot);
        }

        // UI를 업데이트합니다.
        UpdateInventoryUI((ItemType)(tabButtonIndex + 1));
    }

    public int GetTotalItemCountOfItem(ItemData item)
    {
        int totalCount = 0;

        foreach (InventorySlot slot in inventory.GetItemsByType(item.itemEnumType))
        {
            if (slot.data.itemCode == item.itemCode)
            {
                totalCount += slot.itemCount;
            }
        }

        return totalCount;
    }
}
