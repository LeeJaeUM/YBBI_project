using System.Collections;
using UnityEngine;

public class ColorChange : MonoBehaviour
{
    public float _duration = 0.5f;

    [SerializeField]
    private SpriteRenderer _spriteRenderer;
    [SerializeField]
    private Color _originColor;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _originColor = _spriteRenderer.color;
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    StopAllCoroutines();
        //    StartCoroutine(ChangeColor());
        //}
    }

    IEnumerator ChangeColor()
    {
        _spriteRenderer.color = _originColor * new Color(253.0f, 161.0f, 161.0f, 255);

        yield return new WaitForSeconds(_duration);

        _spriteRenderer.color = _originColor;
    }
}
