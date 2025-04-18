using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Slot : MonoBehaviour
{
    [SerializeField]
    public Item _item;
    [SerializeField]
    bool _isSelected;
    [SerializeField]
    Inventory _inventory;

    public Item _storedItem;

    public bool IsSelected { get { return _isSelected; } set { _isSelected = value; } }

    public bool IsEmpty { get { return _item == null; } }

    public void InitSlot(Inventory inventory)
    {
        _inventory = inventory;
    }

    public void SetSlot(Item item)
    {
        _item = item;
        _storedItem = _item;
        _item.transform.SetParent(transform);
        _item.transform.localPosition = Vector3.zero;
        _item.transform.localScale = Vector3.one;
    }

    public Item GetItem() 
    { 
        return _storedItem;
    }

    public void UseItem()
    {
        if (_item == null)
        {
            return;
        }

        var itemInfo = _item.GetItemInfo();

        if (itemInfo._data._property != ItemProperty.Use)
        {
            Debug.LogWarning("이 아이템은 사용할 수 없는 타입입니다.");
            return;
        }

        var result = _item.Use();

        if (result == 0)
        {
            _item = null;
            _inventory.CleanInventorySlots();
        }
    }

    public void OnSelect()
    {
        _inventory.OnSlotSelect(this);
    }
}
