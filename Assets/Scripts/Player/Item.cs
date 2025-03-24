using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    [SerializeField]
    Image _icon;
    [SerializeField]
    public TextMeshProUGUI _count;
    [SerializeField]
    ItemInfo _itemInfo;

    ItemController _controller;

    public void SetItem(Inventory inven, ItemInfo itemInfo)
    {
        _itemInfo = itemInfo;
        _icon = inven.GetIcon(_itemInfo._data._icon);
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
