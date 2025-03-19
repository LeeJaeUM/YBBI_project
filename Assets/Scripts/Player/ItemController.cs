using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    readonly static float _endYpos = -6f;
    readonly static float _baseDration = 1.5f;
    [SerializeField]
    SpriteRenderer _iconSprite;
    [SerializeField]
    float _duration = 1f;
    [SerializeField]
    AnimationCurve _yCurve = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField]
    AnimationCurve _xCurve = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField]
    Vector3 _from;
    [SerializeField]
    Vector3 _to;

    bool _isMagnet;
    public ItemManager.ItemType Type;

    IEnumerator CoPlayTween()
    {
        float time = 0f;
        float valueY = 0;
        float valueX = 0;
        Vector3 yPos = Vector3.zero;
        Vector3 xPos = Vector3.zero;
        _duration = _baseDration;// * (_to - _from).magnitude / (new Vector3(0, 6f) - new Vector3(0, _endYpos)).magnitude;
        while (true)
        {
            if (time > 1f)
            {
                transform.position = _to;
                ItemManager.Instance.Remove(this);
                yield break;
            }
            valueY = _yCurve.Evaluate(time);
            valueX = _xCurve.Evaluate(time);
            yPos = _from * (1f - valueY) + _to * valueY;
            xPos = _from * (1f - valueX) + _to * valueX;
            transform.position = new Vector3(xPos.x, yPos.y);
            time += Time.deltaTime / _duration;
            yield return null;
        }
    }
    public void SetItem(Vector3 position, ItemManager.ItemType type)
    {
        transform.localRotation = Quaternion.identity;
        var dir = new Vector3(Random.Range(-1, 2), 0);
        _from = position;
        _to = new Vector3(_from.x + dir.x * 0.5f, _endYpos);
        StopAllCoroutines();
        StartCoroutine(CoPlayTween());
        Type = type;
        _iconSprite.sprite = ItemManager.Instance.GetIcon(type);
        if (Type >= ItemManager.ItemType.fire && Type <= ItemManager.ItemType.water)
        {
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ItemManager.Instance.Remove(this);
            switch (Type)
            {
                case ItemManager.ItemType.fire:
                    break;
                case ItemManager.ItemType.water:
                    break;
                case ItemManager.ItemType.None:
                    // ItemManager.Instance._player.SetBuff(BuffType.Invincible);
                    break;
                case ItemManager.ItemType.Max:
                    // ItemManager.Instance._player.SetBuff(BuffType.Magnet);
                    break;

            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }
}