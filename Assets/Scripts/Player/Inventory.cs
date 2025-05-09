using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 인벤토리 시스템을 관리하는 클래스
/// 아이템 슬롯 생성, UI 업데이트, 아이템 사용 등의 기능을 제공
/// </summary>
public class Inventory : MonoBehaviour
{
    #region Fields

    [Header("Prefabs & Assets")]
    [SerializeField]
    GameObject _slotPrefab;     // 슬롯 프리팹
    [SerializeField]
    GameObject _itemPrefab;     // 아이템 프리팹
    [SerializeField]
    ItemDataTable _itemTable;   // 아이템 데이터 테이블
    [SerializeField]
    public Sprite[] _iconSprites;   // 아이콘 스프라이트 배열

    [Header("UI Elements")]
    [SerializeField]
    GridLayoutGroup _slotGrid;      // 슬롯 그리드 레이아웃
    [SerializeField]
    Image _cursorSprite;    // 커서 스프라이트 이미지

    [Header("Inventory Slots")]
    [SerializeField]
    public List<Slot> _slotList = new List<Slot>();     // 슬롯 리스트

    #endregion

    #region Properties

    /// <summary>
    /// 인벤토리가 열려 있는지 여부를 반환
    /// </summary>
    public bool IsOpened { get { return gameObject.activeSelf; } }

    #endregion

    #region Utility Methods

    /// <summary>
    /// 주어진 인덱스에 해당하는 아이콘 스프라이트를 반환
    /// </summary>
    /// /// <param name="index">아이콘의 인덱스</param>
    /// <returns>아이콘 스프라이트</returns>
    public Sprite GetIcon(int index)
    {
        return _iconSprites[index];
    }

    #endregion

    #region UI Control

    /// <summary>
    /// 인벤토리 UI를 활성화
    /// </summary>
    public void ShowUI()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// 인벤토리 UI를 비활성화
    /// </summary>
    public void CloseUI()
    {
        SetCursorNull();
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 커서 스프라이트를 비활성화하여 숨김
    /// </summary>
    public void SetCursorNull()
    {
        _cursorSprite.enabled = false;
    }

    #endregion

    #region Slot Interaction Methods

    /// <summary>
    /// 슬롯을 선택하여 커서를 해당 슬롯으로 이동
    /// </summary>
    /// <param name="currentSlot">선택된 슬롯</param>
    public void OnSlotSelect(Slot current_slot)
    {
        var prevSlot = _slotList.Find(slot => slot.IsSelected);

        if (prevSlot != null)
        {
            prevSlot.IsSelected = false;
        }
        _cursorSprite.enabled = true;
        current_slot.IsSelected = true;
        _cursorSprite.transform.position = current_slot.transform.position;
    }

    /// <summary>
    /// 현재 선택된 슬롯의 아이템을 사용
    /// </summary>
    public void OnItemUse()
    {
        var curslot = _slotList.Find(slot => slot.IsSelected);
        if (curslot != null)
        {
            curslot.UseItem();
        }
    }

    #endregion

    #region Slot & Item Management Methods

    /// <summary>
    /// 지정된 수의 슬롯을 생성하여 인벤토리에 추가
    /// </summary>
    /// <param name="count">생성할 슬롯의 수</param>
    public void CreateSlot(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var obj = Instantiate(_slotPrefab);
            obj.transform.SetParent(_slotGrid.transform);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one;
            var slot = obj.GetComponent<Slot>();
            slot.InitSlot(this);
            _slotList.Add(slot);
        }
    }

    /// <summary>
    /// 지정된 인덱스에 해당하는 아이템을 인벤토리에 추가
    /// </summary>
    /// <param name="index">아이템의 인덱스</param>
    public void CreateItem(int index)
    {
        for (int i = 0; i < _slotList.Count; i++)
        {
            var curSlot = _slotList[i];

            if (curSlot.IsEmpty)
            {
                var obj = Instantiate(_itemPrefab);
                var type = (ItemType)index;
                int count = 1;
                var item = obj.GetComponent<Item>();
                ItemInfo itemInfo = new ItemInfo() { _data = _itemTable[type], _count = count };
                item.SetItem(this, itemInfo);
                curSlot.SetSlot(item);
                return; 
            }
            else
            {
                var existingItem = curSlot.GetItem(); 
                if (existingItem != null && existingItem.ItemType == (ItemType)index)
                { 
                    existingItem.IncreaseCount();
                    return;
                }
            }
        }

        // 만약 슬롯 리스트에 빈 공간이 없다면, 필요에 따라 추가적인 처리
        Debug.Log("No empty slots available!");
    }

    /// <summary>
    /// 판매 가능한 아이템을 인벤토리에서 제거
    /// </summary>
    public void RemoveSellItems()
    {
        foreach (var slot in _slotList)
        {
            var item = slot.GetItem();

            if (item != null)
            {
                var itemInfo = item.GetItemInfo();

                if (itemInfo._data._property == ItemProperty.Sell)
                {
                    Destroy(item.gameObject);
                }
            }
        }
    }

    /// <summary>
    /// 인벤토리 슬롯을 정리하여 빈 슬롯을 앞쪽으로 이동
    /// </summary>
    public void CleanInventorySlots()
    {
        int itemIndex = 0;

        for (int i = 0; i < _slotList.Count; i++)
        {
            if (!_slotList[i].IsEmpty)
            {
                if (i != itemIndex)
                {
                    var item = _slotList[i].GetItem();
                    _slotList[itemIndex].SetSlot(item);
                    _slotList[i]._item = null;
                    _slotList[i]._storedItem = null;
                }

                itemIndex++;
            }
        }
    }

    /// <summary>
    /// 인벤토리 슬롯의 수를 증가
    /// </summary>
    public void InvenUpgrade()
    {
        CreateSlot(3);
    }

    #endregion

    #region Unity Events

    /// <summary>
    /// 게임 시작 시 초기화 작업을 수행
    /// </summary>
    void Start()
    {
        if(_itemTable == null)
        {
            _itemTable = FindAnyObjectByType<ItemDataTable>();
        }
        _iconSprites = ItemManager.Instance._itemSprites;
        CreateSlot(10);
        _cursorSprite.enabled = false;
        gameObject.SetActive(false);    
    }

    /// <summary>
    /// 인벤토리 UI가 활성화될 때 슬롯을 정리
    /// </summary>
    private void OnEnable()
    {
        CleanInventorySlots();
    }

    #endregion
}
