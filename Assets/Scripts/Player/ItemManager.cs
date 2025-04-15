using System.Collections;
using System.Collections.Generic;
using UnityEditor.Playables;
using UnityEngine;
using UnityEngine.UI;

public class ItemManager : SingletonMonoBehaviour<ItemManager>
{
    #region Serialized Fields
    [Header("Prefab & Inventory")]
    [SerializeField]
    GameObject _itemPrefab;
    [SerializeField]
    Inventory inventory;

    [Header("Sprites")]
    [SerializeField]
    public Sprite[] _itemSprites;

    [Header("UI References")]
    public RectTransform _itemCreatePos;
    public RectTransform _canvasPos;
    #endregion

    #region Private Fields 
    GameObjectPool<ItemController> _pool;
    float[] _itemTable = { 5f, 15f, 20f, 25f, 35f };
    #endregion

    #region Item Creation & Pooling
    public void Create(RectTransform position)
    {
        ItemType type = (ItemType)Utility.GetProbability(_itemTable);
        var item = _pool.Get();
        item.SetItem(type);
        item.gameObject.SetActive(true);
    }

    // 아이템 생성 방식을 적이 사망한 위치에서 생성할 때 쓰는 함수, 위의 Create 함수를 주석 처리하고 이 함수를 사용하면 됨
    //public void Create(Vector3 worldPosition)
    //{
    //    ItemType type = (ItemType)Utility.GetProbability(_itemTable);
    //    var item = _pool.Get();
    //    item.SetItem(type);

    //    Camera uiCamera = Camera.main;

    //    Vector2 uiPosition;
    //    RectTransformUtility.ScreenPointToLocalPointInRectangle(
    //        _canvasPos,
    //        Camera.main.WorldToScreenPoint(worldPosition),
    //        uiCamera,
    //        out uiPosition);

    //    RectTransform itemRect = item.GetComponent<RectTransform>();
    //    itemRect.anchoredPosition = uiPosition;

    //    item.gameObject.SetActive(true);
    //}

    public void Remove(ItemController item)
    {
        item.gameObject.SetActive(false);
        _pool.Set(item);
    }

    void SetRandomPosition()
    {
        Rect canvasRect = _canvasPos.rect;

        float randomX = Random.Range(canvasRect.xMin, canvasRect.xMax);
        float randomY = Random.Range(canvasRect.yMin, canvasRect.yMax);

        _itemCreatePos.anchoredPosition = new Vector2(randomX, randomY);
    }
    #endregion

    #region Inventory Handling
    public Sprite GetIcon(ItemType type)
    {
        return _itemSprites[(int)type];
    }

    public void SetInvenItem(int index)
    {
        inventory.CreateItem(index);
    }

    public void SetInvenItemZero()
    {
        inventory.SetCursorNull();   
    }

    public void ResetInvenSlots()
    {
        inventory.CleanInventorySlots();
    }
    #endregion

    #region Unity Methods
    protected override void OnAwake()
    {
        _itemSprites = Resources.LoadAll<Sprite>("Items/");
        _pool = new GameObjectPool<ItemController>(1, () =>
        {
            var obj = Instantiate(_itemPrefab);
            obj.SetActive(false);
            obj.transform.SetParent(transform);
            SetRandomPosition();
            obj.transform.position.Equals(_itemCreatePos.anchoredPosition);
            var item = obj.GetComponent<ItemController>();
            return item;
        });
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            SetRandomPosition();
            Create(_itemCreatePos);
        }
    }
    #endregion
}