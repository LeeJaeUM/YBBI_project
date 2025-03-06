using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Item : MonoBehaviour
{
    [SerializeField]
    Image _icon;
    [SerializeField]
    Label _count;
    [SerializeField]
    ItemInfo _itemInfo;

    public void SetItem(Inventory inven, ItemInfo itemInfo)
    {
        _itemInfo = itemInfo;
        _icon.sprite = inven.GetIcon(_itemInfo._data._icon);
        SetCount();
    }

    public int Use()
    {
        _itemInfo._count--;
        SetCount();
        if (_itemInfo._count <= 0)
        {
            Destroy(gameObject);
        }

        return _itemInfo._count;
    }

    void SetCount()
    {
        _count.text = _itemInfo._count.ToString();
    }
}
