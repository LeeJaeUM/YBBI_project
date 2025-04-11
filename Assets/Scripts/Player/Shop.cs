using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    [SerializeField]
    TMP_Text _playerMoneyText;
    [SerializeField]
    Inventory _inventory;
    [SerializeField]
    List<ShopItem> _shopItems;

    private int _playerMoney = 0;
    private int _totalPrice = 0;

    public void ShowShopUI()
    {
        gameObject.SetActive(true);
    }

    public void CloseShopUI()
    {
        gameObject.SetActive(false);
    }

    public void OnSellButtonPressed()
    {
        CalculateTotalPrice();
        _playerMoney += _totalPrice;
        _playerMoneyText.text = _playerMoney.ToString();
        _inventory.RemoveSellItems();
        UpdateBtnStates();
    }

    public void UpdatePriceDisplay()
    {
        // CalculateTotalPrice();
        _playerMoneyText.text = _playerMoney.ToString();
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

    void UpdateBtnStates()
    {
        foreach (var item in _shopItems)
        {
            item.UpdateButtonState(_playerMoney);
        }
    }

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
}
