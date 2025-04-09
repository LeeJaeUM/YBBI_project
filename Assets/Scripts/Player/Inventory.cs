using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class Inventory : MonoBehaviour
{
    [SerializeField]
    GameObject _slotPrefab;
    [SerializeField]
    GameObject _itemPrefab;
    [SerializeField]
    ItemDataTable _itemTable;  
    [SerializeField]
    GridLayoutGroup _slotGrid;
    [SerializeField]
    Image _cursorSprite;
    [SerializeField]
    public Sprite[] _iconSprites;
    [SerializeField]
    public List<Slot> _slotList = new List<Slot>();

    public bool IsOpened { get { return gameObject.activeSelf; } }

    public Sprite GetIcon(int index)
    {
        return _iconSprites[index];
    }

    public void ShowUI()
    {
        gameObject.SetActive(true);
    }

    public void CloseUI()
    {
        SetCursorNull();
        gameObject.SetActive(false);
    }

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

    public void OnItemUse()
    {
        var curslot = _slotList.Find(slot => slot.IsSelected);
        if (curslot != null)
        {
            curslot.UseItem();
        }
    }

    public void SetCursorNull()
    {
        _cursorSprite.enabled = false;
    }

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

    public void InvenUpgrade()
    {
        CreateSlot(5);
    }

    private void OnEnable()
    {
        CleanInventorySlots();
    }

    // Start is called before the first frame update
    void Start()
    {
        _iconSprites = ItemManager.Instance._itemSprites;
        CreateSlot(10);
        _cursorSprite.enabled = false;
        gameObject.SetActive(false);    
    }
}
