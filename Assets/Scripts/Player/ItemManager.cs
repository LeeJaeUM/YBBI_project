using System.Collections;
using System.Collections.Generic;
using UnityEditor.Playables;
using UnityEngine;
using UnityEngine.UI;

public class ItemManager : SingletonMonoBehaviour<ItemManager>
{
    [SerializeField]
    public PlayerItemController _player;

    ItemType _itemType;

    [SerializeField]
    GameObject _itemPrefab;
    GameObjectPool<ItemController> _pool;
    [SerializeField]
    Image[] _iconImages;
    float[] _itemTable = { 5f, 15f, 20f, 25f, 35f };

    public void Create(Vector3 position)
    {
        ItemType type = (ItemType)Utility.GetProbability(_itemTable);
        var item = _pool.Get();
        item.gameObject.SetActive(true);
        item.SetItem(type);
    }

    public void Remove(ItemController item)
    {
        item.gameObject.SetActive(false);
        _pool.Set(item);
    }

    public Image GetIconImage(ItemType type)
    {
        return _iconImages[(int)type];
    }

    // Start is called before the first frame update
    protected override void OnStart()
    {
        _iconImages = Resources.LoadAll<Image>("Items/");
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