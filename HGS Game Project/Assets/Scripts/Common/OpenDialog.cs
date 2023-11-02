using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class OpenDialog : MonoBehaviour
{
    public InputField txtInputItemCount;
    public Button btnOK;
    public Button btnCancel;

    private ShopManager shopManager;

    private void Awake()
    {
        shopManager = FindObjectOfType<ShopManager>();
        CloseDialog();
    }

    private void Start()
    {
        btnOK.onClick.AddListener(OnOkButtonClicked);
        btnCancel.onClick.AddListener(OnCancelButtonClicked);
    }

    private void OnOkButtonClicked()
    {
        int itemPrice;

        if (!int.TryParse(ShopContent.currentSelectedShopContent.ItemData.itemPurchasePrice, out itemPrice))
        {
            Debug.LogError("Invalid item price format!");
            return;
        }

        int enteredValue = 1; // 기본값 설정
        if (txtInputItemCount != null && txtInputItemCount.gameObject.activeSelf)
        {
            if (!int.TryParse(txtInputItemCount.text, out enteredValue))
            {
                Debug.LogError("Invalid input! Please enter a valid number.");
                return;
            }

            if (enteredValue < 1 || enteredValue > 200)
            {
                Debug.LogError("Invalid input! Please enter a number between 1 and 200.");
                return;
            }
        }

        if (ShopContent.currentSelectedShopContent.isSellingItem)
        {
            // 중첩 가능한 아이템의 판매 로직
            //Debug.Log("입력된 수량: " + enteredValue);
            shopManager.SellItem(ShopContent.currentSelectedShopContent.ItemData, enteredValue);
        }
        else
        {
            // 중첩 가능한 아이템의 구매 로직
            if (shopManager.CanAfford(enteredValue * itemPrice))
            {
                shopManager.BuyItem(ShopContent.currentSelectedShopContent.ItemData, enteredValue);
            }
            else
            {
                Debug.LogError("Not enough money!");
            }
        }

        CloseDialog();
    }

    private void OnCancelButtonClicked()
    {
        Debug.Log("Transaction cancelled");
        CloseDialog();
    }

    public void ToggleInputField(bool isActive)
    {
        if (txtInputItemCount != null)
        {
            txtInputItemCount.gameObject.SetActive(isActive);
        }
    }

    public void SetFocusToInputField()
    {
        if (txtInputItemCount != null)
        {
            shopManager.isInputFieldFocused = true;
            txtInputItemCount.ActivateInputField();
            txtInputItemCount.Select();
        }
    }

    public void RemoveFocusFromInputField()
    {
        if (txtInputItemCount != null)
        {
            shopManager.isInputFieldFocused = false;
            txtInputItemCount.DeactivateInputField();
        }
    }

    private void CloseDialog()
    {
        gameObject.SetActive(false);
        RemoveFocusFromInputField();
    }
}
