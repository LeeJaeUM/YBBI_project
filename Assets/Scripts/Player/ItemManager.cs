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
    GameObject _itemPrefab;     // 아이템을 생성할 때 사용할 프리팹
    [SerializeField]
    Inventory inventory;        // 아이템을 관리할 인벤토리 참조

    [Header("Sprites")]
    [SerializeField]
    public Sprite[] _itemSprites;       // 아이템 종류에 해당하는 스프라이트 배열

    [Header("UI References")]
    public RectTransform _canvasPos;        // UI 캔버스의 위치
    public RectTransform _itemCreatePos;        // 아이템 생성 위치 (UI 상에서)

    #endregion

    #region Private Fields 

    GameObjectPool<ItemController> _pool;       // 아이템 풀을 위한 객체 풀
    float[] _itemTable = { 8f, 8f, 8f, 8f, 8f, 7f, 7f, 6f, 5f, 5f, 5f, 6f, 6f, 6f, 6f, 1f };      // 각 아이템의 생성 확률에 해당하는 테이블  

    #endregion

    #region Item Creation & Pooling

    /// <summary>
    /// 아이템을 지정된 위치에 생성하는 함수
    /// </summary>
    public void Create(RectTransform position)
    {
        // 아이템 생성 확률에 따라 아이템 타입을 결정
        ItemType type = (ItemType)Utility.GetProbability(_itemTable);

        // 객체 풀에서 아이템을 가져옴
        var item = _pool.Get();

        // 아이템에 타입 설정
        item.SetItem(type);

        // 아이템을 활성화
        item.gameObject.SetActive(true);
    }

    // 아이템 생성 방식을 적이 사망한 위치에서 생성할 때 쓰는 함수, 위의 Create 함수를 주석 처리하고 이 함수를 사용하면 됨
    //public void Create(Vector3 worldPosition)
    //{
    //    // 아이템 생성 확률에 따라 아이템 타입을 결정
    //    ItemType type = (ItemType)Utility.GetProbability(_itemTable);

    //    // 객체 풀에서 아이템을 가져옴
    //    var item = _pool.Get();
    //    item.SetItem(type);

    //    // 월드 위치를 UI 위치로 변환
    //    Camera uiCamera = Camera.main;

    //    Vector2 uiPosition;
    //    RectTransformUtility.ScreenPointToLocalPointInRectangle(
    //        _canvasPos,
    //        Camera.main.WorldToScreenPoint(worldPosition),
    //        uiCamera,
    //        out uiPosition);

    //    // UI 상에서 아이템 위치 설정
    //    RectTransform itemRect = item.GetComponent<RectTransform>();
    //    itemRect.anchoredPosition = uiPosition;

    //    // 아이템을 활성화
    //    item.gameObject.SetActive(true);
    //}

    /// <summary>
    /// 객체 풀에서 아이템을 제거하여 비활성화하는 함수
    /// </summary>
    public void Remove(ItemController item)
    {
        item.gameObject.SetActive(false);       // 아이템을 비활성화
        _pool.Set(item);        // 객체 풀에 아이템을 반환
    }

    /// <summary>
    /// 아이템이 랜덤한 위치에 배치되도록 설정하는 함수
    /// </summary>
    void SetRandomPosition()
    {
        Rect canvasRect = _canvasPos.rect;      // 캔버스의 크기 정보를 가져옴

        // X, Y 값의 랜덤 위치를 설정
        float randomX = Random.Range(canvasRect.xMin, canvasRect.xMax);
        float randomY = Random.Range(canvasRect.yMin, canvasRect.yMax);

        // 아이템 생성 위치를 랜덤값으로 설정
        _itemCreatePos.anchoredPosition = new Vector2(randomX, randomY);
    }

    #endregion

    #region Inventory Handling

    /// <summary>
    /// 주어진 아이템 타입에 대한 아이콘을 반환하는 함수
    /// </summary>
    public Sprite GetIcon(ItemType type)
    {
        return _itemSprites[(int)type];     // 아이템 타입에 해당하는 스프라이트를 반환
    }

    /// <summary>
    /// 인벤토리에서 특정 인덱스의 아이템을 생성하는 함수
    /// </summary>
    public void SetInvenItem(int index)
    {
        inventory.CreateItem(index);        // 인벤토리에서 아이템을 생성
    }

    /// <summary>
    /// 인벤토리 아이템을 초기화 (커서 상태를 초기화)하는 함수
    /// </summary>
    public void SetInvenItemZero()
    {
        inventory.SetCursorNull();      // 커서를 초기 상태로 설정
    }

    /// <summary>
    /// 인벤토리 슬롯을 초기화하는 함수
    /// </summary>
    public void ResetInvenSlots()
    {
        inventory.CleanInventorySlots();       // 인벤토리 슬롯을 깨끗하게 청소 
    }

    #endregion

    #region Unity Methods

    /// <summary>
    /// 초기 설정: 아이템 프리팹, 풀, UI 설정 등을 초기화
    /// </summary>
    protected override void OnAwake()
    {
        _itemSprites = Resources.LoadAll<Sprite>("Items/");     // 아이템 스프라이트를 Resources 폴더에서 로드
        _pool = new GameObjectPool<ItemController>(1, () =>
        {
            // 객체 풀 초기화: 아이템 프리팹을 인스턴스화하여 풀에 추가
            var obj = Instantiate(_itemPrefab);
            obj.SetActive(false);       // 비활성화 상태로 생성
            obj.transform.SetParent(transform);     // 부모를 현재 객체로 설정
            SetRandomPosition();        // 랜덤한 위치 설정
            obj.transform.position.Equals(_itemCreatePos.anchoredPosition);     // 위치 지정
            var item = obj.GetComponent<ItemController>();      // ItemController 컴포넌트 가져오기
            return item;        // 생성된 아이템을 반환
        });
    }

    /// <summary>
    /// 매 프레임마다 호출: 특정 키 입력에 따라 아이템을 생성
    /// </summary>
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