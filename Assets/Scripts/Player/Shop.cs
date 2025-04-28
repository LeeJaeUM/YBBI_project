using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField]
    TMP_Text _playerMoneyText;      // 플레이어 소지금 표시 텍스트

    [Header("Dependencies")]
    [SerializeField]
    Inventory _inventory;       // 인벤토리 참조
    PlayerATKStats _playerATKStats;     // 플레이어 공격력 스탯 참조
    PlayerMover _playerMover;       // 플레이어 이동 속도 스탯 참조
    UnitHealth _unitHealth;     // 플레이어 공기 감소 스탯 참조

    [Header("Shop Items")]
    [SerializeField]
    List<ShopItem> _shopItems;      // 상점에서 판매하는 아이템 목록

    private int _playerMoney = 100000;      // 초기 플레이어 소지금
    private int _totalPrice = 0;        // 판매 금액 총합

    #region UI Control

    /// <summary>
    /// 상점 UI를 화면에 표시
    /// </summary>
    public void ShowShopUI()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// 상점 UI를 화면에서 숨김
    /// </summary>
    public void CloseShopUI()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 플레이어의 현재 소지금을 UI에 표시
    /// </summary>
    public void UpdatePriceDisplay()
    {
        _playerMoneyText.text = _playerMoney.ToString();
    }

    /// <summary>
    /// 아이템 구매가능하면 버튼 활성화
    /// </summary>
    void UpdateBtnStates()
    {
        foreach (var item in _shopItems)
        {
            item.UpdateButtonState(_playerMoney);
        }
    }

    #endregion

    #region Sell Logic

    /// <summary>
    /// 판매 버튼 클릭 시 호출. 판매 가능한 아이템을 계산하고 소지금을 증가
    /// </summary>
    public void OnSellButtonPressed()
    {
        CalculateTotalPrice();      // 판매 금액 계산
        _playerMoney += _totalPrice;        // 소지금 추가
        _playerMoneyText.text = _playerMoney.ToString();        // UI 갱신
        _inventory.RemoveSellItems();       // 인벤토리에서 판매 아이템 제거
        UpdateBtnStates();      // 버튼 상태 갱신
    }

    /// <summary>
    /// 인벤토리 내 판매 가능한 아이템의 총 가격을 계산
    /// </summary>
    private void CalculateTotalPrice()
    {
        _totalPrice = 0;

        foreach (var slot in _inventory._slotList)
        {
            var item = slot.GetItem();

            if (item != null) 
            {
                var itemInfo = item.GetItemInfo();

                if (itemInfo._data._property == ItemProperty.Sell)
                {
                    _totalPrice += item.GetItemInfo()._data._price * item.GetItemInfo()._count;
                }
            }
        }
    }

    #endregion

    #region Purchase Logic

    /// <summary>
    /// 아이템 구매 버튼 클릭 시 호출. 구매 조건이 충족되면 아이템을 구매
    /// </summary>
    /// <param name="item">구매 대상 ShopItem</param>
    public void OnItemBuyButtonPressed(ShopItem item)
    {
        if (item._IsBuyItem(ref _playerMoney))
        {
            UpdatePriceDisplay();       // 소지금 UI 갱신
            UpdateBtnStates();      // 버튼 상태 갱신
        }
        else
        {
            Debug.Log("자본이 부족하여 구매할 수 없습니다.");
        }
    }

    #endregion

    #region Gamble Logic

    /// <summary>
    /// 돈을 도박처럼 70% 확률로 절반, 30% 확률로 두 배로 변경하는 함수
    /// </summary>
    public void GamblePlayerMoney()
    {
        // 0~1 사이의 랜덤 값 생성
        float randomChance = Random.Range(0f, 1f);

        if (randomChance <= 0.7f)
        {
            // 70% 확률로 소지금을 절반으로
            _playerMoney = Mathf.FloorToInt(_playerMoney * 0.5f);
        }
        else
        {
            // 30% 확률로 소지금을 두 배로
            _playerMoney = Mathf.FloorToInt(_playerMoney * 2f);
        }

        // UI 갱신
        UpdatePriceDisplay();
    }

    #endregion

    #region Unity Methods

    /// <summary>
    /// 초기 설정: UI 초기화, 버튼 리스너 설정, 구매 이벤트 정의
    /// </summary>
    void Start()
    {
        UpdatePriceDisplay();       // 소지금 초기 표시
        gameObject.SetActive(false);        // 상점 UI 비활성화

        foreach (var item in _shopItems)
        {
            item.Initialize();      // ShopItem 초기화

            ShopItem currentItem = item;
            item._buyBtn.onClick.AddListener(() => OnItemBuyButtonPressed(currentItem));        // 버튼 이벤트 바인딩
        }

        // 각 아이템 구매 시 실행될 추가 동작 정의
        _shopItems[0]._onPurchase = () => _inventory.InvenUpgrade();
        _shopItems[1]._onPurchase = () => _playerATKStats.ShopATKUpgrade();
        _shopItems[2]._onPurchase = () => _playerMover.ShopMoveSpeedUpgrade();
        _shopItems[3]._onPurchase = () => _unitHealth.ShopMaxAirUpgrade();
        _shopItems[4]._onPurchase = () => _unitHealth.ShopBaseMinusAirPerSecUpgrade();

        UpdateBtnStates();      // 버튼 초기 상태 설정
    }

    #endregion  
}