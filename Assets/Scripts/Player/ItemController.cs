using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ItemController : MonoBehaviour
{
    [SerializeField]
    Image _iconImage;
    
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

            ItemManager.Instance.SetInvenItem(_index);
            ItemManager.Instance.Remove(this); 
        }
    }

    void Awake()
    {
        
    }
}