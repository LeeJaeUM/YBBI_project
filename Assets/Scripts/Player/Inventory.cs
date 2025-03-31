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
    Image _cursorSprite;
    [SerializeField]
    GridLayoutGroup _slotGrid;
    [SerializeField]
    public Sprite[] _iconSprites;
    [SerializeField]
    List<Slot> _slotList = new List<Slot>();

    public bool IsOpened { get { return gameObject.activeSelf; } }

    public Sprite GetIcon(int index)
    {
        foreach (var a in _iconSprites)
        {
            Debug.Log("ddd");
        }
        return _iconSprites[index];
    }

    public void ShowUI()
    {
        gameObject.SetActive(true);
    }

    public void CloseUI()
    {
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
        // 슬롯 리스트의 각 슬롯을 순차적으로 확인
        for (int i = 0; i < _slotList.Count; i++)
        {
            var curSlot = _slotList[i];

            // 슬롯이 비어있다면 새로운 아이템 생성
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


    public void InvenUpgrade()
    {
        CreateSlot(5);
    }

    void Awake()
    {
        
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
