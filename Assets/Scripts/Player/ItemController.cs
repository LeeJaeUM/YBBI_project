using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;
using UnityEngine.UI;

public class ItemController : MonoBehaviour
{
    [SerializeField]
    Image _iconImage;
    Inventory Inventory;
    //[SerializeField]
    //AnimationCurve _yCurve = AnimationCurve.Linear(0, 0, 1, 1);
    //[SerializeField]
    //AnimationCurve _xCurve = AnimationCurve.Linear(0, 0, 1, 1);

    public int _index;

    public void SetItem(ItemType _type)
    {
        var type = _type;
        _iconImage.sprite = ItemManager.Instance.GetIcon(type);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _index = (int.Parse(_iconImage.sprite.name.Split('.')[0]) - 1);            
            ItemManager.Instance.Remove(this);
            Inventory.CreateItem(_index);
        }
    }
}