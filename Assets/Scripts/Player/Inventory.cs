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
    ItemController ItemCtrl;

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
        var curSlot = _slotList.Find((_slot) => _slot.IsEmpty);

        if ( curSlot != null)
        {
            var obj = Instantiate(_itemPrefab);
            var type = (ItemType)index;
            int count = 1;
            var item = obj.GetComponent<Item>();
            ItemInfo itemInfo = new ItemInfo() { _data = _itemTable[type], _count = count };
            item.SetItem(this, itemInfo);
            curSlot.SetSlot(item);
        }
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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            // CreateItem();
        }
    }
}
