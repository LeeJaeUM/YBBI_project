using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    #region Fields

    [SerializeField]
    Image _icon;    // 아이템 아이콘 이미지
    [SerializeField]
    public TextMeshProUGUI _count;      // 아이템 개수 표시 텍스트
    [SerializeField]
    ItemInfo _itemInfo;     // 아이템 정보

    #endregion

    #region Properties

    // 아이템 타입 (읽기 전용)
    public ItemType ItemType { get; private set; }

    #endregion

    #region Public Methods

    /// <summary>
    /// 아이템 정보를 설정하고 아이콘 및 개수 UI를 업데이트
    /// </summary>
    public void SetItem(Inventory inven, ItemInfo itemInfo)
    {
        _itemInfo = itemInfo;
        _icon.sprite = inven.GetIcon(_itemInfo._data._icon);    // 아이콘 설정
        ItemType = _itemInfo._data._id;     // 아이템 타입 설정
        SetCount();     // 개수 표시
    }

    /// <summary>
    /// 아이템을 사용하여 개수를 줄이고, 0개가 되면 오브젝트를 파괴
    /// </summary>
    public int Use()
    {
        _itemInfo._count--;
        SetCount();

        if (_itemInfo._count <= 0)
        {
            Destroy(gameObject);    // 아이템 오브젝트 파괴
            ItemManager.Instance.SetInvenItemZero();    // 아이템 매니저에 빈 아이템 알림
        }
        return _itemInfo._count;
    }

    /// <summary>
    /// 아이템 개수를 1 증가시키고 UI를 업데이트
    /// </summary>
    public void IncreaseCount()
    {
        _itemInfo._count++;
        SetCount();
    }

    /// <summary>
    /// 아이템 가격을 반환
    /// </summary>
    public int GetPrice()
    {
        return _itemInfo._data._price;
    }

    /// <summary>
    /// 아이템 정보를 반환
    /// </summary>
    public ItemInfo GetItemInfo()
    {
        return _itemInfo;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// 아이템 개수를 UI 텍스트로 설정
    /// </summary>
    void SetCount()
    {
        _count.text = _itemInfo._count.ToString();
    }

    #endregion
}
