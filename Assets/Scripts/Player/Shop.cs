using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    [SerializeField]
    TMP_Text _priceText;
    [SerializeField]
    Inventory _inventory;

    private int totalPrice = 0;

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
        _priceText.text = totalPrice.ToString();
        _inventory.RemoveAllItems();
    }

    private void CalculateTotalPrice()
    {
        foreach (var slot in _inventory._slotList)
        {
            var item = slot.GetItem();

            if (item != null) 
            {
                var itemInfo = item.GetItemInfo();

                if (itemInfo._data._property == ItemProperty.Sell)
                {
                    totalPrice += item.GetItemInfo()._data._price * item.GetItemInfo()._count;
                }
            }
        }
    }

    public void UpdatePriceDisplay()
    {
        CalculateTotalPrice();
        _priceText.text = totalPrice.ToString();
    }
    
    void Start()
    {
        UpdatePriceDisplay();
        gameObject.SetActive(false);
    }
}
