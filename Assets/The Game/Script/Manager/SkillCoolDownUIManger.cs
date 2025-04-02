using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem.OnScreen;
using static Enums;
using UnityEngine.EventSystems;

public class SkillCoolDownUIManager : MonoBehaviour
{
    public Enums.SkillType btnType;

    public Image _coolDownImg;
    public Button _baseAtkBtn;
    public TextMeshProUGUI _coolDownText;

    [SerializeField] private float _coolDown = 5.0f;
    private bool _isCoolDown = false;
    private bool _isButtonHeld = false;  // 버튼을 꾹 눌렀는지 여부

    public PlayerATKStats _playerATKStats;
    public TheGamePlayerInputHandler _inputHandler;

    /// <summary>
    /// PlayerInput의 Attack과 연결되는 컴포넌트 (GamePad의 westBtn)
    /// </summary>
    [SerializeField] private OnScreenButton _screenButton;

    // 스킬 버튼 클릭 시 호출되는 함수
    void OnClickButton()
    {
        if (!_isCoolDown)
        {
            // 코루틴을 통해 쿨타임 관리 시작
            StartCoroutine(CooldownCoroutine());
        }
    }

    void OnPressedButton(bool isPressed)
    {
        Debug.Log($"{isPressed} 지금 이 상태다");
        if (!_isCoolDown)
        {
            // 코루틴을 통해 쿨타임 관리 시작
            StartCoroutine(CooldownCoroutine());
        }
    }

    // 코루틴으로 쿨타임 관리
    IEnumerator CooldownCoroutine()
    {
        _isCoolDown = true;
        //_screenButton.enabled = false;
        float timer = _coolDown;

        // 버튼 비활성화
        _baseAtkBtn.interactable = false;
        _coolDownText.enabled = true;

        while (timer > 0)
        {
            timer -= Time.deltaTime;

            // 쿨타임 UI 업데이트
            _coolDownText.text = timer.ToString("F2");

            if (_coolDownImg != null)
            {
                _coolDownImg.fillAmount = 1f - (timer / _coolDown);
            }

            yield return null; // 매 프레임마다 반복
        }

        // 쿨타임 종료 후
        _isCoolDown = false;
        // _screenButton.enabled = true;
        _coolDownText.enabled = false;
        _baseAtkBtn.interactable = true;

        if (_coolDownImg != null)
        {
            _coolDownImg.fillAmount = 1f; // 쿨타임 종료 후 이미지 채우기
        }
    }

    private void Update()
    {
        if (_isButtonHeld)
        {

        }
    }

    private void Awake()
    {
        if (_inputHandler != null)   // 이벤트 구독
        {
            switch (btnType)
            {
                case Enums.SkillType.NONE:
                    Debug.Log("공격버튼에 아무것도 할당되지 않음");
                    break;
                case Enums.SkillType.Normal:
                    _inputHandler.OnAttackInput += OnPressedButton;
                    break;
                case Enums.SkillType.Pressure:
                    _inputHandler.OnPressureSkillInput += OnClickButton;
                    break;
                case Enums.SkillType.Unique:
                    _inputHandler.OnUniqueSkillInput += OnClickButton;
                    break;
            }
        }
    }

    private void OnDestroy()
    {
        if (_inputHandler != null)   // 이벤트 구독
        {
            switch (btnType)
            {
                case Enums.SkillType.NONE:
                    Debug.Log("공격버튼에 아무것도 할당되지 않음");
                    break;
                case Enums.SkillType.Normal:
                    _inputHandler.OnAttackInput -= OnPressedButton;
                    break;
                case Enums.SkillType.Pressure:
                    _inputHandler.OnPressureSkillInput -= OnClickButton;
                    break;
                case Enums.SkillType.Unique:
                    _inputHandler.OnUniqueSkillInput -= OnClickButton;
                    break;
            }
        }

    }
    void Start()
    {
    }

    public void SetUpStart(PlayerATKStats playerAtkStats)
    {
        Debug.Log("셋업 시작");
        _baseAtkBtn.onClick.AddListener(OnClickButton);
        _coolDownText.enabled = false;

        if (_coolDownImg != null)
        {
            _coolDownImg.fillAmount = 1;
        }
        _playerATKStats = playerAtkStats.GetComponent<PlayerATKStats>();
        switch (btnType)
        {
            case Enums.SkillType.NONE:
                Debug.Log("공격버튼에 아무것도 할당되지 않음");
                break;
            case Enums.SkillType.Normal:
                _coolDown = _playerATKStats.GetNormalCoolDown();
                break;
            case Enums.SkillType.Pressure:
                _coolDown = _playerATKStats.GetPressureSkillCoolDown();
                break;
            case Enums.SkillType.Unique:
                _coolDown = _playerATKStats.GetUniqueSkillCoolDown();
                break;
        }

        _screenButton = GetComponent<OnScreenButton>();
        _coolDownText = GetComponentInChildren<TextMeshProUGUI>();
    }

    //public void OnPointerDown(PointerEventData eventData)
    //{
    //    // 버튼을 꾹 눌렀을 때
    //    _isButtonHeld = true;
    //    if (!_isCoolDown)
    //    {
    //        Attack(); // 공격 실행
    //        StartCoroutine(CooldownCoroutine()); // 쿨타임 시작
    //    }
    //}

    //public void OnPointerUp(PointerEventData eventData)
    //{
    //    // 버튼에서 손을 뗐을 때
    //    _isButtonHeld = false;
    //}
}