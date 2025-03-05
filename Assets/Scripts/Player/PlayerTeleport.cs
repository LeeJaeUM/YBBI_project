using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerTeleport : MonoBehaviour
{
    public List<Vector2> _teleportPos;
    public string _targetTag = "Player";
    public string _triggerTag = "TeleportArea";

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(_targetTag) && gameObject.CompareTag(_triggerTag))
        {
            Debug.Log("Coll");
            TeleportPlayer(collision.transform);
        }
    }

    void TeleportPlayer (Transform playerTransform)
    {
        if (_teleportPos.Count > 0)
        {
            int randomIndex = Random.Range (0, _teleportPos.Count);
            playerTransform.position = _teleportPos[randomIndex];
        }
        else
        {
            Debug.Log("좌표 없음");
        }
    }
}
