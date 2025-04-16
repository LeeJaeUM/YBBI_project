using UnityEngine;

public class TheGameShopUIController : MonoBehaviour
{
    [SerializeField]
    TheGameShop _shop;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _shop.ShowShopUI();
        }
    } 
}
