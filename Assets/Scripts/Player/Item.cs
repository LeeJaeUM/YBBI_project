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

    public ItemType ItemType { get; private set; }

    public void SetItem(Inventory inven, ItemInfo itemInfo)
    {
        _itemInfo = itemInfo;
        _icon.sprite = inven.GetIcon(_itemInfo._data._icon);
        ItemType = _itemInfo._data._id;
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

    public void IncreaseCount()
    {
        _itemInfo._count++;
        SetCount();
    }
}
