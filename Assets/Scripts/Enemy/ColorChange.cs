using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class ColorChange : MonoBehaviour
{
    public float _duration = 0.1f;

    [SerializeField]
    private SpriteRenderer _spriteRenderer;
    [SerializeField]
    private Color _originColor;

    private UnitHealth _unitHealth;

    public void DamageEffect()
    {
        StopAllCoroutines();
        StartCoroutine(ChangeColor());
    }

    IEnumerator ChangeColor()
    {
        _spriteRenderer.color = _originColor * new Color(253.0f, 161.0f, 161.0f, 255);

        yield return new WaitForSeconds(_duration);

        _spriteRenderer.color = _originColor;
    }

    void Start()
    {
        _unitHealth = GetComponent<UnitHealth>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _originColor = _spriteRenderer.color;

        _unitHealth.OnDamage += DamageEffect;
    }

    void OnDisable()
    {
        _unitHealth.OnDamage -= DamageEffect;
    }


}
