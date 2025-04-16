using UnityEngine;

public class TheGameShopUIController : MonoBehaviour
{
    [SerializeField]
    Shop _shop;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _shop.ShowShopUI();
        }
    } 
}
