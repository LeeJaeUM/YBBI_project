using UnityEngine;

public class TheGameShopUIController : MonoBehaviour
{

    private TheGameShop _shop;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _shop.ShowShopUI();
        }
    }

    private void Awake()
    {
        _shop = FindAnyObjectByType<TheGameShop>();
    }
}
