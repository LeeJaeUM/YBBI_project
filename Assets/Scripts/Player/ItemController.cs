using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ItemController : MonoBehaviour
{
    #region Fields

    [SerializeField]
    Image _iconImage;       // 아이템의 아이콘 이미지 (UI에 사용)

    public int _index;      // 인벤토리에서의 아이템 인덱스

    #endregion

    #region Public Methods

    /// <summary>
    /// 아이템 타입을 받아 아이콘 이미지를 설정
    /// </summary>
    public void SetItem(ItemType _type)
    {
        var type = _type;
        _iconImage.sprite = ItemManager.Instance.GetIcon(type);     // 아이콘 설정
    }

    #endregion

    #region Unity Methods

    /// <summary>
    /// 플레이어와 충돌 시 인벤토리에 아이템 추가 및 해당 아이템 제거 처리
    /// </summary>
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // 스프라이트 이름에서 인덱스를 추출 (예: "3.png" → 3 → 인덱스 2)
            _index = (int.Parse(_iconImage.sprite.name.Split('.')[0]) - 1);

            ItemManager.Instance.SetInvenItem(_index);      // 인벤토리에 아이템 추가
            ItemManager.Instance.Remove(this);      // 필드에서 해당 아이템 제거
        }
    }

    #endregion
}