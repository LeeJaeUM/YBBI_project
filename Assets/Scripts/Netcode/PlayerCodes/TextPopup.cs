using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class TextPopup : MonoBehaviour
{
    [SerializeField] private float _destroyTime = 5f; // 삭제될 시간 (기본값 5초)
    [SerializeField] private TextMeshPro _tmpText = new TextMeshPro();

    private ulong _playerId;


    private void Start()
    { 
        _tmpText.text = $"ClientId :{_playerId}";
        Destroy(gameObject, _destroyTime); // 지정된 시간이 지나면 오브젝트 삭제
    }

    public void SetId(ulong id)
    {
        _playerId = id;
    }

}
