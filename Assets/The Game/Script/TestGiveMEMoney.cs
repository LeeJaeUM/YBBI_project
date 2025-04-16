using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestGiveMeMoney : TestBase
{
    [Header("상점 오브젝트(ShopBgImg)")]
    [SerializeField] GameObject _shopOBJ;

    private TheGameShop _shop;

    private void Start()
    {
        _shop = _shopOBJ.GetComponent<TheGameShop>();
    }


    public override void Test1(InputAction.CallbackContext context)
    {
        _shop.AddMoney(10);
    }
}
