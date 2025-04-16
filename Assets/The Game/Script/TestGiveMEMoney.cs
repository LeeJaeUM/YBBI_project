using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestGiveMeMoney : TestBase
{
    [Header("상점 오브젝트(ShopBgImg)")]
    [SerializeField] GameObject _shopOBJ;

    private Shop _shop;

    private void Start()
    {
        _shop = _shopOBJ.GetComponent<Shop>();
    }


    public override void Test1(InputAction.CallbackContext context)
    {
        
    }
}
