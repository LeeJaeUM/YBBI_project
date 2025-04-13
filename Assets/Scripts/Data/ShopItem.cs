using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ShopItem
{
    #region Item Data
    public string _itemName;
    public int _initialPrice;
    public float _priceIncreaseRate;
    #endregion

    #region UI References
    public TMP_Text _priceText;
    public Button _buyBtn;
    #endregion

    #region Internal State
    private int _curPrice;

    public Action _onPurchase;
    #endregion

    #region Initialization
    public void Initialize()
    {
        _curPrice = _initialPrice;
        UpdatePriceText();
    }
    #endregion

    #region Price Logic
    public int GetPrice()
    {
        return _curPrice;
    }

    public void IncreasePrice()
    {
        _curPrice = Mathf.FloorToInt(_curPrice * _priceIncreaseRate);
        UpdatePriceText();
    }

    public void UpdatePriceText()
    {
        if (_priceText != null)
        {
            _priceText.text = "- " + _curPrice.ToString();
        }
    }
    #endregion

    #region Purchase Logic
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
    #endregion

    #region Button State
    public void UpdateButtonState(int playerMoney)
    {
        if (_buyBtn != null) 
        {
            _buyBtn.interactable = (playerMoney >= _curPrice);
        }
    }
    #endregion
}
