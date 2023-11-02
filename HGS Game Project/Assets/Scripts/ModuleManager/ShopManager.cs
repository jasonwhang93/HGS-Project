using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface INPC
{
    bool IsSpriteDirLeft { get; }
    List<string> ShopItemCodeList { get; }
}


public class ShopManager : MonoBehaviour
{
    public GameObject shopUI;

    public Transform contentsTransform; // Contents 오브젝트의 Transform
    private GameObject shopContentPrefab; // Shop Content Prefab

    public ItemDatabase itemDB;
    private INPC npcScript;

    [HideInInspector] public GameObject shopManagerNPC;
    public Image shopManagerNPCImage;
    private LithHarborShop_UsableItem lithHarborShopUsableItemList;

    public Transform playerContentsTransform; // Contents 오브젝트의 Transform
    [SerializeField] private Button[] tabButton;
    private int currentTabIndex = 1;

    public InventoryManager playerInventory;

    public Text playerMoney;

    // 아이템을 구매하거나 판매할 때 한 번 더 안내해주기 위한 공지박스
    public GameObject buyCountNoticeBox;
    public GameObject buyNoticeBox;
    public GameObject SellCountNoticeBox;
    public GameObject sellNoticeBox;

    [HideInInspector] public OpenDialog buyDialog;
    [HideInInspector] public OpenDialog sellDialog;

    [HideInInspector] public bool isInputFieldFocused = false;

    // Start 메서드 내에 아래 코드를 추가합니다:
    public Button btnBuyItem; // Inspector에서 "Area :: BtnBuyItem" 버튼을 연결해주세요.
    public Button btnSellItem; // Inspector에서 "Area :: BtnBuyItem" 버튼을 연결해주세요.

    private void Awake()
    {
        shopContentPrefab = Resources.Load<GameObject>("Prefabs/UI/Shop Content");

        currentTabIndex = 1;
    }

    private void Start()
    {
        // 게임 시작 시 상점 UI를 비활성화
        CloseShopUI();

        if (btnBuyItem != null)
        {
            btnBuyItem.onClick.AddListener(OnBuyAndSellItemButtonClicked);
        }

        if (btnSellItem != null)
        {
            btnSellItem.onClick.AddListener(OnBuyAndSellItemButtonClicked);
        }

        EventManager.instance.AddEvent("Shop :: UIToggle", (p) =>
        {
            // 인벤토리 창의 활성화 상태를 토글
            shopUI.SetActive(!shopUI.activeInHierarchy);

            if (shopUI.activeInHierarchy)
            {
                // NPC 정보 저장 및 이미지 및 스크립트 설정
                if (p != null && p is GameObject)
                {
                    shopManagerNPC = p as GameObject;
                    SpriteRenderer npcSprite = shopManagerNPC.GetComponent<SpriteRenderer>();
                    if (npcSprite != null)
                    {
                        shopManagerNPCImage.sprite = npcSprite.sprite;

                        // NPC 이미지 방향 설정
                        npcScript = shopManagerNPC.GetComponent<INPC>();

                        if (npcScript != null)
                        { 
                            // 부모 객체인 Area :: ShopNPC UI의 localScale을 변경
                            shopManagerNPCImage.transform.parent.localScale = npcScript.IsSpriteDirLeft
                                ? new Vector3(-1, 1, 1)
                                : new Vector3(1, 1, 1);
                        }
                    }
                }

                OpenShopUI();
                UpdateShopUI();
                tabButton[currentTabIndex].Select();
                UpdatePlayerMoneyUI();
            }
        });

        EventManager.instance.AddEvent("OpenBuyCountNoticeBox", (p) =>
        {
            buyCountNoticeBox.SetActive(true);
            buyCountNoticeBox.GetComponent<OpenDialog>().SetFocusToInputField(); // 포커스 설정
        });

        EventManager.instance.AddEvent("OpenBuyNoticeBox", (p) =>
        {
            buyNoticeBox.SetActive(true);
        });

        EventManager.instance.AddEvent("OpenSellCountNoticeBox", (p) =>
        {
            SellCountNoticeBox.SetActive(true);
            SellCountNoticeBox.GetComponent<OpenDialog>().SetFocusToInputField(); // 포커스 설정
        });

        EventManager.instance.AddEvent("OpenSellNoticeBox", (p) =>
        {
            sellNoticeBox.SetActive(true);
        });
    }

    private void Update()
    {
        if (shopUI.activeInHierarchy && !isInputFieldFocused)
        {
            tabButton[currentTabIndex].Select();
        }

    }

    // "아이템 사기" 버튼 클릭 시 호출될 메서드
    public void OnBuyAndSellItemButtonClicked()
    {
        if (ShopContent.currentSelectedShopContent != null)
        {
            ShopContent selectedContent = ShopContent.currentSelectedShopContent;

            EventManager.instance.SendEvent("Shop CountNoticeBox :: UIToggle");

            string eventName = "";
            if (selectedContent.isSellingItem) // 판매인 경우
            {
                eventName = selectedContent.ItemData.itemIsStackable ? "OpenSellCountNoticeBox" : "OpenSellNoticeBox";
            }
            else // 구매인 경우
            {
                eventName = selectedContent.ItemData.itemIsStackable ? "OpenBuyCountNoticeBox" : "OpenBuyNoticeBox";
            }
            EventManager.instance.SendEvent(eventName);
        }
    }

    public void ClickEquipmentTab()
    {
        currentTabIndex = 0;

        UpdateShopUI();
    }

    public void ClickUseableTab()
    {
        currentTabIndex = 1;

        UpdateShopUI();
    }

    public void ClickOtherTab()
    {
        currentTabIndex = 2;

        UpdateShopUI();
    }

    public void ClickSetUpTab()
    {
        currentTabIndex = 3;

        UpdateShopUI();
    }

    public void ClickCashTab()
    {
        currentTabIndex = 4;

        UpdateShopUI();
    }

    public void OpenShopUI()
    {
        // 기존에 추가된 상점 아이템들 삭제
        foreach (Transform child in contentsTransform)
        {
            Destroy(child.gameObject);
        }

        // 상점 UI를 활성화
        shopUI.SetActive(true);

        foreach (string itemCode in npcScript.ShopItemCodeList)
        {
            ItemData itemData = itemDB.GetItemByCode(itemCode);

            if (itemData != null)
            {
                // Shop Content Prefab을 생성하고 Contents 오브젝트의 자식으로 추가
                GameObject shopContentObject = Instantiate(shopContentPrefab, contentsTransform);

                // ShopContent 스크립트 가져오기
                ShopContent shopContent = shopContentObject.GetComponent<ShopContent>();

                if (shopContent != null)
                {
                    // 아이템 정보 설정 (구매를 위한 것이므로 isSelling을 false로 설정)
                    shopContent.SetItemData(itemData, false);
                }
                else
                {
                    Debug.LogError("ShopContent 스크립트를 찾을 수 없습니다.");
                }
            }
        }
    }

    public void UpdateShopUI()
    {
        ClearAllShopContents();

        foreach (InventorySlot slot in playerInventory.inventory.GetItemsByType((ItemType)(currentTabIndex + 1)))
        {
            ItemData itemData = slot.data;

            // Item_Null이 아닌 아이템만 목록에 추가
            if (itemData != null && !(itemData is Item_Null))
            {
                // Shop Content Prefab을 생성하고 Contents 오브젝트의 자식으로 추가
                GameObject shopContentObject = Instantiate(shopContentPrefab, playerContentsTransform);

                // ShopContent 스크립트 가져오기
                ShopContent shopContent = shopContentObject.GetComponent<ShopContent>();

                if (shopContent != null)
                {
                    // 아이템 정보 설정 (판매를 위한 것이므로 isSelling을 true로 설정)
                    shopContent.SetItemData(itemData, true);

                    // 아이템 수량 설정
                    shopContent.SetItemCount(slot, false); // 여기서는 판매 목록이 아니므로 false를 전달합니다.
                }
                else
                {
                    Debug.LogError("ShopContent 스크립트를 찾을 수 없습니다.");
                }
            }
        }
    }

    private void ClearAllShopContents()
    {
        foreach (Transform content in playerContentsTransform)
        {
            Destroy(content.gameObject);
        }
    }

    // 소지금 UI 업데이트하는 메서드
    public void UpdatePlayerMoneyUI()
    {
        if (playerMoney != null)
        {
            playerMoney.text = string.Format("{0:N0}", PlayerData.playerMoney);
        }
    }

    public void CloseShopUI()
    {
        shopUI.SetActive(false);
    }

    private void OpenCountNoticeBox()
    {
        buyCountNoticeBox.SetActive(true);
    }

    // 사용자의 소지금과 아이템의 가격을 비교하여 구매가 가능한지 확인하는 함수
    public bool CanAfford(int price)
    {
        return PlayerData.playerMoney >= price;
    }


    // 아이템을 구매할 때 호출할 메서드 등 추가적인 기능 구현 가능
    public void BuyItem()
    {
        if (ShopContent.currentSelectedShopContent != null && ShopContent.currentSelectedShopContent.ItemData != null)
        {
            ItemData itemToBuy = ShopContent.currentSelectedShopContent.ItemData;

            if (itemToBuy.itemIsStackable)
            {
                // 중첩 가능한 아이템 구매 로직
                buyDialog.ToggleInputField(true); // 수량 입력 필드 활성화
                buyDialog.gameObject.SetActive(true);
            }
            else
            {
                // 중첩 불가능한 아이템 구매 로직
                buyDialog.ToggleInputField(false); // 수량 입력 필드 비활성화
                buyDialog.gameObject.SetActive(true);
            }
        }
        else
        {
            Debug.LogError("구매할 아이템을 선택해주세요!");
        }
    }

    // 아이템을 구매하는 함수
    public void BuyItem(ItemData itemToBuy, int quantity)
    {
        int itemPrice;
        if (int.TryParse(itemToBuy.itemPurchasePrice, out itemPrice))
        {
            int totalCost = itemPrice * quantity;

            // 소지금이 충분한 경우
            if (CanAfford(totalCost))
            {
                // 소지금에서 아이템 가격을 차감
                PlayerData.playerMoney -= totalCost;

                // 인벤토리에 아이템 추가
                for (int i = 0; i < quantity; i++)
                {
                    playerInventory.AddItem(itemToBuy);
                }

                // UI 업데이트
                UpdatePlayerMoneyUI();
                UpdateShopUI();
            }
            else
            {
                Debug.LogError("소지금이 부족합니다!");
            }
        }
        else
        {
            Debug.LogError("아이템 가격 정보가 잘못되었습니다!");
        }
    }

    public void SellItem(ItemData itemToSell, int quantity)
    {
        int itemSellPrice;

        // 플레이어가 가진 해당 아이템의 총 수량 확인
        int totalOwned = playerInventory.GetTotalItemCountOfItem(itemToSell);

        if (totalOwned < quantity)
        {
            Debug.LogError("판매하려는 수량이 보유 수량보다 많습니다!");
            return; // 판매 수량이 보유 수량보다 많으면 로직 중단
        }

        if (int.TryParse(itemToSell.itemSellPrice, out itemSellPrice))
        {
            int totalEarned = itemSellPrice * quantity;

            // 소지금에 아이템 판매 가격을 추가
            PlayerData.playerMoney += totalEarned;

            // 인벤토리에서 아이템 제거
            for (int i = 0; i < quantity; i++)
            {
                playerInventory.AddItem(itemToSell, -1);
            }

            // UI 업데이트
            UpdatePlayerMoneyUI();
            UpdateShopUI();
        }
        else
        {
            Debug.LogError("아이템 판매 가격 정보가 잘못되었습니다!");
        }
    }

}
