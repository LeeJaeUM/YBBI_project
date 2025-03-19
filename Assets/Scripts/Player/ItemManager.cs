using System.Collections;
using System.Collections.Generic;
using UnityEditor.Playables;
using UnityEngine;

public class ItemManager : SingletonMonoBehaviour<ItemManager>
{
    [SerializeField]
    public PlayerItemController _player;

    public enum ItemType
    {
        None,
        fire,
        water,
        Max
    }

    [SerializeField]
    GameObject _itemPrefab;
    GameObjectPool<ItemController> _pool;
    [SerializeField]
    Sprite[] _iconSprites;
    float[] _itemTable = { 100f };

    public void Create(Vector3 position)
    {
        // ItemType type = (ItemType)Utility.GetProbability(_itemTable);
        var item = _pool.Get();
        item.gameObject.SetActive(true);
        // item.SetItem(position, type);
    }
    public void Remove(ItemController item)
    {
        item.gameObject.SetActive(false);
        _pool.Set(item);
    }

    public Sprite GetIcon(ItemType type)
    {
        return _iconSprites[(int)type];
    }

    // Start is called before the first frame update
    protected override void OnStart()
    {
        _iconSprites = Resources.LoadAll<Sprite>("Items/");
        _pool = new GameObjectPool<ItemController>(5, () =>
        {
            var obj = Instantiate(_itemPrefab);
            obj.SetActive(false);
            obj.transform.SetParent(transform);
            var item = obj.GetComponent<ItemController>();
            return item;
        });
    }

}