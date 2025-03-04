using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class TextPopup : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private float destroyTime = 5f; // 삭제될 시간 (기본값 5초)
    [SerializeField] private TextMeshPro _tmpText = new TextMeshPro();

    private void Start()
    {
        _tmpText.text = $"ClientId : {PlayerHandler._staticPlayerId}";
        Destroy(gameObject, destroyTime); // 지정된 시간이 지나면 오브젝트 삭제
    }
}
