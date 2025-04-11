using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ShopItem
{
    public string _itemName;
    public int _initialPrice;
    public float _priceIncreaseRate;

    public TMP_Text _priceText;
    public Button _buyBtn;

    private int _curPrice;

    public Action _onPurchase;

    public void Initialize()
    {
        _curPrice = _initialPrice;
        UpdatePriceText();
    }

    public void UpdatePriceText()
    {
        if (_priceText != null)
        {
            _priceText.text = "- " + _curPrice.ToString();
        }
    }

    public void IncreasePrice()
    {
        _curPrice = Mathf.FloorToInt(_curPrice * _priceIncreaseRate);
        UpdatePriceText();
    }

    public int GetPrice()
    {
        return _curPrice;
    }

    public bool _IsBuyItem (ref int playerMoney)
    {
        if (playerMoney >= _curPrice)
        {
            playerMoney -= _curPrice;
            IncreasePrice();
            UpdatePriceText();

            _onPurchase?.Invoke();

            return true;
        }

        return false;
    }

    public void UpdateButtonState(int playerMoney)
    {
        if (_buyBtn != null) 
        {
            _buyBtn.interactable = (playerMoney >= _curPrice);
        }
    }
}
