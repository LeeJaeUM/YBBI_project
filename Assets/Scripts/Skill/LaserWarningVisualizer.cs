using System.Collections;
using UnityEngine;

public class LaserWarningVisualizer : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] private SpriteRenderer _warningSpriteRenderer;
    [SerializeField] private SpriteRenderer _timerSpriteRenderer;

    /// <summary>
    /// 레이저 경고를 표시
    /// </summary>
    /// <param name="spawnPosition">스폰위치</param>
    /// <param name="direction">방향</param>
    /// <param name="width">두께</param>
    /// <param name="length">길이 </param>
    /// <param name="duration">경고시간</param>
    public void ShowLaserWarning(Vector3 spawnPosition, Vector3 direction, float width, float length, float duration)
    {
        // 회전/위치는 부모가 처리하니까 무시

        // WarningSprite는 최종 공격 범위로 처음부터 고정
        _warningSpriteRenderer.transform.localScale = new Vector3(width, length, 1f);
        _warningSpriteRenderer.transform.localPosition = new Vector3(0, length / 2f, 0); // 중심에서 위로

        // TimerSprite는 길이 0으로 시작해서 점점 커짐
        _timerSpriteRenderer.transform.localScale = new Vector3(width, 0f, 1f);
        _timerSpriteRenderer.transform.localPosition = new Vector3(0, 0f, 0);

        _warningSpriteRenderer.enabled = true;
        _timerSpriteRenderer.enabled = true;

        //// 부모 콜라이더 설정
        //BoxCollider2D parentCollider = GetComponentInParent<BoxCollider2D>();
        //if (parentCollider != null)
        //{
        //    parentCollider.size = new Vector2(width, length);
        //    parentCollider.offset = new Vector2(0, length / 2f);
        //}

        StartCoroutine(GrowTimer(length, duration));
    }

    private IEnumerator GrowTimer(float maxLength, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float currentLength = Mathf.Lerp(0, maxLength, t);
            _timerSpriteRenderer.transform.localScale = new Vector3(_timerSpriteRenderer.transform.localScale.x, currentLength, 1f);
            _timerSpriteRenderer.transform.localPosition = new Vector3(0, currentLength / 2f, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }

        _timerSpriteRenderer.transform.localScale = new Vector3(_timerSpriteRenderer.transform.localScale.x, maxLength, 1f);
        _timerSpriteRenderer.transform.localPosition = new Vector3(0, maxLength / 2f, 0);

        HideWarning();
    }

    public void ShowBombWarning(float duration)
    {
        _warningSpriteRenderer.enabled = true;
        _timerSpriteRenderer.enabled = true;

        StartCoroutine(Bomb_GrowTimer(1, duration));
    }

    private IEnumerator Bomb_GrowTimer(float maxLength, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float currentLength = Mathf.Lerp(0, maxLength, t);
            _timerSpriteRenderer.transform.localScale = new Vector3(_timerSpriteRenderer.transform.localScale.x, currentLength, 1f);
            elapsed += Time.deltaTime;
            yield return null;
        }

        _timerSpriteRenderer.transform.localScale = new Vector3(_timerSpriteRenderer.transform.localScale.x, maxLength, 1f);

        HideWarning();
    }

    public void HideWarning()
    {
        _warningSpriteRenderer.enabled = false;
        _timerSpriteRenderer.enabled = false;
    }


    private void Awake()
    {
        Transform child = transform.GetChild(0);
        _warningSpriteRenderer = child.GetComponent<SpriteRenderer>();

        child = transform.GetChild(1);
        _timerSpriteRenderer = child.GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        _warningSpriteRenderer.enabled = false;
        _timerSpriteRenderer.enabled = false;
    }
}
