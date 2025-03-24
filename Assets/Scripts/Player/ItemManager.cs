using System.Collections;
using System.Collections.Generic;
using UnityEditor.Playables;
using UnityEngine;
using UnityEngine.UI;

public class ItemManager : SingletonMonoBehaviour<ItemManager>
{
    [SerializeField]
    public PlayerItemController _player;

    [SerializeField]
    GameObject _itemPrefab;
    GameObjectPool<ItemController> _pool;
    [SerializeField]
    Sprite[] _itemSprites;
    public RectTransform _itemCreatePos;
    public RectTransform _canvasPos;
    float[] _itemTable = { 5f, 15f, 20f, 25f, 35f };
    
    public void Create(RectTransform position)
    {
        ItemType type = (ItemType)Utility.GetProbability(_itemTable);
        var item = _pool.Get(); 
        item.SetItem(type);
        item.gameObject.SetActive(true);
    }

    public void Remove(ItemController item)
    {
        item.gameObject.SetActive(false);
        _pool.Set(item);
    }

    public Sprite GetIcon(ItemType type)
    {
        return _itemSprites[(int)type];
    }
    
    void SetRandomPosition()
    {
        Rect canvasRect = _canvasPos.rect;

        float randomX = Random.Range(canvasRect.xMin, canvasRect.xMax);
        float randomY = Random.Range(canvasRect.yMin, canvasRect.yMax);

        _itemCreatePos.anchoredPosition = new Vector2(randomX, randomY);
    }

    // Start is called before the first frame update
    protected override void OnStart()
    {
        _itemSprites = Resources.LoadAll<Sprite>("Items/");
        _pool = new GameObjectPool<ItemController>(5, () =>
        {
            var obj = Instantiate(_itemPrefab);
            obj.SetActive(false);
            obj.transform.SetParent(transform);
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
}