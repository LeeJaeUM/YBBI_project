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
    Grid _slotGrid;
    [SerializeField]
    Sprite[] _iconSprites;
    //[SerializeField]
    //TweenScale _tweenOpen;
    //[SerializeField]
    //TweenScale _tweenClose;
    [SerializeField]
    List<Slot> _slotList = new List<Slot>();

    public bool IsOpened { get { return gameObject.activeSelf; } }

    public Sprite GetIcon(int index)
    {
        return _iconSprites[index];
    }

    public void ShowUI()
    {
        //_tweenOpen.ResetToBeginning();
        //_tweenOpen.PlayForward();
        gameObject.SetActive(true);
    }
    public void HideUI()
    {
        //_tweenClose.enabled = true;
        //_tweenClose.ResetToBeginning();
        //_tweenClose.PlayForward();
    }
    public void CloseUI()
    {
        gameObject.SetActive(false);
        //_tweenClose.enabled = false;
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
        // _slotGrid.repositionNow = true;
    }

    public void CreatItem()
    {
        var curSlot = _slotList.Find((slot) => slot.IsEmpty);
        if (curSlot != null)
        {
            var obj = Instantiate(_itemPrefab);
            var type = (ItemType)Random.Range(0, (int)ItemType.Max);
            int count = Random.Range(1, 11);
            ItemInfo itemInfo = new ItemInfo() { _data = _itemTable[type], _count = count };
            var item = obj.GetComponent<Item>();
            item.SetItem(this, itemInfo);
            curSlot.SetSlot(item);
        }
    }

    void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        CreateSlot(24);
        _cursorSprite.enabled = false;
        gameObject.SetActive(false);
        //_tweenClose.enabled = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            CreateSlot(1);
        }
    }
}
