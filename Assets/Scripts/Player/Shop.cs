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
        int totalPrice = CalculateTotalPrice();
        _priceText.text = "Total Price: " + totalPrice.ToString();
        _inventory.RemoveAllItems();
    }

    private int CalculateTotalPrice()
    {
        int totalPrice = 0;

        foreach (var slot in _inventory._slotList)
        {
            var item = slot.GetItem();
            if (item != null) 
            {
                totalPrice += item.GetItemInfo()._data._price * item.GetItemInfo()._count;
            }
        }

        return totalPrice;
    }

    public void UpdatePriceDisplay()
    {
        int totalPrice = CalculateTotalPrice();
        _priceText.text = "Total Price: " + totalPrice.ToString();
    }
    
    void Start()
    {
        UpdatePriceDisplay();
        gameObject.SetActive(false);
    }
}
