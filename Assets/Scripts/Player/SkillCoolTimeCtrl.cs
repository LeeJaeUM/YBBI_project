using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem.OnScreen;

public class SkillCoolTimeCtrl : MonoBehaviour
{
    public Image _coolTimeImg;
    public Button _baseAtkBtn;
    public TextMeshProUGUI _coolTimeText;
    [SerializeField] private float _coolTime = 5.0f;

    private bool _isCoolTime = false;
    
    public PlayerATKStats _playerATKStats;

    /// <summary>
    /// PlayerInput의 Attack과 연결되는 컴포넌트 (GamePad의 westBtn)
    /// </summary>
    [SerializeField] private OnScreenButton _screenButton; 

    // 스킬 버튼 클릭 시 호출되는 함수
    void OnClickButton()
    {
        if (!_isCoolTime)
        {
            // Attack 함수 호출
            Attack();

            // 코루틴을 통해 쿨타임 관리 시작
            StartCoroutine(CooldownCoroutine());
        }
    }

    // 공격 함수
    void Attack()
    {
        // 실제 공격 로직 처리
        Debug.Log("스킬 공격 실행!");
    }

    // 코루틴으로 쿨타임 관리
    IEnumerator CooldownCoroutine()
    {
        _isCoolTime = true;
        _screenButton.enabled = false;
        float timer = _coolTime;

        // 버튼 비활성화
        _baseAtkBtn.interactable = false;
        _coolTimeText.gameObject.SetActive(true);

        while (timer > 0)
        {
            timer -= Time.deltaTime;

            // 쿨타임 UI 업데이트
            _coolTimeText.text = timer.ToString("F2");

            if (_coolTimeImg != null)
            {
                _coolTimeImg.fillAmount = 1f - (timer / _coolTime);
            }

            yield return null; // 매 프레임마다 반복
        }

        // 쿨타임 종료 후
        _isCoolTime = false;
        _screenButton.enabled = true;
        _coolTimeText.gameObject.SetActive(false);
        _baseAtkBtn.interactable = true;

        if (_coolTimeImg != null)
        {
            _coolTimeImg.fillAmount = 1f; // 쿨타임 종료 후 이미지 채우기
        }
    }
    void Start()
    {
        _baseAtkBtn.onClick.AddListener(OnClickButton);
        _coolTimeText.gameObject.SetActive(false);

        if (_coolTimeImg != null)
        {
            _coolTimeImg.fillAmount = 1;
        }

        _coolTime = _playerATKStats.GetNormalCoolTime();

        _screenButton = GetComponent<OnScreenButton>();
    }
}
