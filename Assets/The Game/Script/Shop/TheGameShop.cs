using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TheGameShop : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField]
    TMP_Text _playerMoneyText;

    [Header("Dependencies")]
    [SerializeField]
    Inventory _inventory;

    [Header("Shop Items")]
    [SerializeField]
    List<ShopItem> _shopItems;

    private int _playerMoney = 0;
    private int _totalPrice = 0;

    #region UI Control
    public void ShowShopUI()
    {
        gameObject.SetActive(true);
    }

    public void CloseShopUI()
    {
        gameObject.SetActive(false);
    }

    public void UpdatePriceDisplay()
    {
        _playerMoneyText.text = _playerMoney.ToString();
    }

    /// <summary>
    /// 아이템 구매가능하면 버튼 활성화
    /// </summary>
    void UpdateBtnStates()
    {
        foreach (var item in _shopItems)
        {
            item.UpdateButtonState(_playerMoney);
        }
    }
    #endregion

    #region Sell Logic

    public void OnSellButtonPressed()
    {
        CalculateTotalPrice();
        _playerMoney += _totalPrice;
        _playerMoneyText.text = _playerMoney.ToString();
        _inventory.RemoveSellItems();
        UpdateBtnStates();
    }

    private void CalculateTotalPrice()
    {
        _totalPrice = 0;

        foreach (var slot in _inventory._slotList)
        {
            var item = slot.GetItem();

            if (item != null) 
            {
                var itemInfo = item.GetItemInfo();

                if (itemInfo._data._property == ItemProperty.Sell)
                {
                    _totalPrice += item.GetItemInfo()._data._price * item.GetItemInfo()._count;
                }
            }
        }
    }
    #endregion

    #region Purchase Logic
    public void OnItemBuyButtonPressed(ShopItem item)
    {
        if (item._IsBuyItem(ref _playerMoney))
        {
            UpdatePriceDisplay();
            UpdateBtnStates();
        }
        else
        {
            Debug.Log("자본이 부족하여 구매할 수 없습니다.");
        }
    }
    #endregion

    #region Unity Methods
    void Start()
    {
        UpdatePriceDisplay();
        gameObject.SetActive(false);

        foreach (var item in _shopItems)
        {
            item.Initialize();

            ShopItem currentItem = item;
            item._buyBtn.onClick.AddListener(() => OnItemBuyButtonPressed(currentItem));
        }

        _shopItems[0]._onPurchase = () => _inventory.InvenUpgrade();

        UpdateBtnStates();
    }
    #endregion  
}