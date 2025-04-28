using UnityEngine;

public class ShopUIController : MonoBehaviour
{
    #region Fields

    [SerializeField]
    Shop _shop;     // 상점 UI를 제어하는 Shop 스크립트 참조

    #endregion

    #region Unity Methods

    /// <summary>
    /// 플레이어가 상점 트리거 범위에 들어왔을 때 상점 UI를 표시
    /// </summary>
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _shop.ShowShopUI();     // 상점 UI 열기
        }
    }

    #endregion
}
