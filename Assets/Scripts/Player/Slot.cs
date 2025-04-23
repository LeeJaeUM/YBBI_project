using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

/// <summary>
/// 인벤토리 슬롯을 나타내는 클래스
/// 아이템의 설정, 사용, 선택 상태 등을 관리
/// </summary>
public class Slot : MonoBehaviour
{
    #region Fields & Serialized Fields

    [SerializeField]
    public Item _item;  // 슬롯에 배치된 아이템
    [SerializeField]
    bool _isSelected;   // 슬롯의 선택 여부
    [SerializeField]
    Inventory _inventory;   // 이 슬롯이 속한 인벤토리
    [SerializeField]
    PlayerItemController _playerItemController;     // 플레이어의 아이템 사용 및 버프 관리

    // 슬롯에 저장된 아이템을 추적하는 변수
    public Item _storedItem;

    #endregion

    #region Properties

    /// <summary>
    /// 슬롯의 선택 상태를 가져오거나 설정
    /// </summary>
    public bool IsSelected { get { return _isSelected; } set { _isSelected = value; } }

    /// <summary>
    /// 슬롯이 비어 있는지 여부를 확인
    /// </summary>
    public bool IsEmpty { get { return _item == null; } }

    #endregion

    #region Initialization

    /// <summary>
    /// 슬롯을 초기화하고 인벤토리와 연결
    /// </summary>
    /// <param name="inventory">이 슬롯이 속할 인벤토리</param>
    public void InitSlot(Inventory inventory)
    {
        _inventory = inventory;
    }

    #endregion

    #region Slot Management

    /// <summary>
    /// 슬롯에 아이템을 설정하고 해당 아이템을 슬롯에 배치
    /// </summary>
    /// <param name="item">설정할 아이템</param>
    public void SetSlot(Item item)
    {
        _item = item;
        _storedItem = _item;
        _item.transform.SetParent(transform);   // 아이템의 부모를 슬롯으로 설정
        _item.transform.localPosition = Vector3.zero;   // 아이템의 위치를 슬롯의 중앙으로 설정
        _item.transform.localScale = Vector3.one;   // 아이템의 크기를 기본 크기로 설정
    }

    /// <summary>
    /// 슬롯에 저장된 아이템을 반환
    /// </summary>
    /// <returns>슬롯에 저장된 아이템</returns>
    public Item GetItem() 
    { 
        return _storedItem;
    }

    #endregion

    #region Item Usage

    /// <summary>
    /// 아이템을 사용하고, 사용 가능한 타입인지 확인한 후, 사용 결과에 따라 인벤토리를 정리
    /// </summary>
    public void UseItem()
    {
        if (_item == null)
        {
            return;
        }

        var itemInfo = _item.GetItemInfo();

        // 아이템의 속성이 'Use'가 아닌 경우 사용 불가
        if (itemInfo._data._property != ItemProperty.Use)
        {
            Debug.LogWarning("이 아이템은 사용할 수 없는 타입입니다.");
            return;
        }
        else
        {
            var bufftype = itemInfo._data._buff;

            // 플레이어에게 버프 적용
            _playerItemController.SetBuff(bufftype);

            // 아이템 사용 시 결과 처리
            var result = _item.Use();

            // 사용 결과가 0이면 아이템이 소모된 것으로 간주하고 슬롯을 비움
            if (result == 0)
            {
                _item = null;
                _inventory.CleanInventorySlots();   // 인벤토리의 슬롯 정리
            }
        }
    }

    #endregion

    #region Selection

    /// <summary>
    /// 슬롯이 선택되었을 때 호출되어 인벤토리에서 해당 슬롯을 선택하도록 함
    /// </summary>
    public void OnSelect()
    {
        _inventory.OnSlotSelect(this);
    }

    #endregion
}
