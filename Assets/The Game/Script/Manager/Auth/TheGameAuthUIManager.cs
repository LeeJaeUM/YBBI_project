using UnityEngine;
using UnityEngine.UI;
using Firebase.Auth;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class TheGameAuthUIManager : MonoBehaviour
{
    #region Fields & Properties

    [Header("Input Field")]
    [SerializeField] private TMP_InputField _loginEmailInputField;        //로그인 이메일
    [SerializeField] private TMP_InputField _loginPasswordInputField;     //로그인 비밀번호
    [SerializeField] private TMP_InputField _signupEmailInputField;           // 회원가입 이메일
    [SerializeField] private TMP_InputField _signupPasswordInputField;        // 회원가입 비밀번호
    [SerializeField] private TMP_InputField _signupPasswordCheckInputField;   // 비밀번호 체크

    [Header("Button")]
    [SerializeField] private Button _loginButton;
    [SerializeField] private Button _signupButton;
    [SerializeField] private Button _signupEnterButton;
    [SerializeField] private Button _exitSignupButton;
    [SerializeField] private Button _logoutButton;

    [Header("Coroutine Field")]
    [SerializeField] private GameObject _signUpUIObj;           //회원가입 UI 패널
    [SerializeField] private GameObject _failedUIObj;     //로그인 및 회원가입 실패 UI 패널
    [SerializeField] private TextMeshProUGUI _failedText;
    [SerializeField] private TextMeshProUGUI _resultText;   //체크용 텍스트 (마지막 작업 텍스트로 표시)
    #endregion


    #region Custom Functions
    /// <summary>
    /// 회원가입 UI 띄우는 함수
    /// </summary>
    private void OnSignUpClicked()
    {
        _signUpUIObj.SetActive(true);
    }

    /// <summary>
    /// 회원가입 시 패스워드 체크 후 회원 등록
    /// </summary>
    private void OnSignUpEnterClicked()
    {
        string email = _signupEmailInputField.text;
        string password = _signupPasswordInputField.text;
        string passwordCheck = _signupPasswordCheckInputField.text;

        //비어있는 입력공간 확인
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(passwordCheck))
        {
            _resultText.text = "Please fill in all fields.";
            OperationFailed("Please fill in all fields.");
            return;
        }

        // 이메일 형식 확인 (간단한 정규식 사용)
        if (!System.Text.RegularExpressions.Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            _resultText.text = "Invalid email format.";
            OperationFailed("Invalid email format.");
            return;
        }

        // 패스워드 길이 확인 (6자리 이상)
        if (password.Length < 6)
        {
            _resultText.text = "Password must be at least 6 characters long.";
            OperationFailed("Password must be at least 6 characters long.");
            return;
        }

        // 패스워드가 패스워드 체크와 동일한지
        if (password != passwordCheck)
        {
            _resultText.text = "Passwords do not match!";
            OperationFailed("Passwords do not match!");
            return;
        }

        // 회원가입 진행
        TheGameAuthManager.Instance.RegisterUser(email, password,
            (user) =>
            {
                Debug.Log("회원가입 성공");
                _resultText.text = "Sign Up Successful! ID: " + user.UserId;
                _signUpUIObj.SetActive(false); // 회원가입 성공 시 UI 닫기
            },
            (error) =>
            {
                Debug.Log("회원가입 실패");
                _resultText.text = "Sign Up Failed: " + error;
                OperationFailed("Sign Up Failed");
            });
    }

    /// <summary>
    /// 로그인 클릭 시 처리 
    /// </summary>
    public void OnLoginClicked()
    {
        string email = _loginEmailInputField.text;
        string password = _loginPasswordInputField.text;

        TheGameAuthManager.Instance.LoginUser(email, password,
            (user) =>
            {
                Debug.Log("Login Successful! ID: " + user.UserId);
                _resultText.text = "Login Successful! ID: " + user.UserId;
                string name = email.Split('@')[0];
                TheGameAuthManager.Instance.SetPlayerNickName(name);
                SceneMove();
            },
            (error) =>
            {
                Debug.Log("Login Failed: 로그인 실패");
                OperationFailed("Login Failed...");
            });
    }

    /// <summary>
    /// 로그인 및 회원가입 
    /// </summary>
    /// <param name="errorText"></param>
    public void OperationFailed(string errorText)
    {
        _failedText.text = errorText;
        StartCoroutine(ShowFailedUI());
    }
    /// <summary>
    /// 로그아웃 클릭 시 처리
    /// </summary>
    void OnLogoutClicked()
    {
        TheGameAuthManager.Instance.LogoutUser();
        _resultText.text = "Logged Out!";
    }

    /// <summary>
    /// 로그인 성공 시 씬 이동용 함수
    /// </summary>
    void SceneMove()
    {
        Debug.Log("씬 이동 활성화 필요");
        LoginSuccessHideUI();
        SceneManager.LoadScene("Lobby&SessionScene");

    }

    void HideUI()
    {
        _signUpUIObj?.SetActive(false);
        _failedUIObj?.SetActive(false);
    }
   
    void LoginSuccessHideUI()
    {
        Transform child = transform.GetChild(0);
        child.gameObject.SetActive(false);
        child = transform.GetChild(1);
        child.gameObject.SetActive(false);
        HideUI();
    }

    #endregion

    #region Coroutine Methods
    private IEnumerator ShowFailedUI()
    {
        Debug.Log("로그인 실패 코루틴");
        _failedUIObj.SetActive(true);
        yield return new WaitForSeconds(2f);
        _failedUIObj.SetActive(false);
    }
    #endregion

    #region Unity Built-in Functions
    void Start()
    {
        //버튼 이벤트 등록
        _exitSignupButton.onClick.AddListener(HideUI);
        _signupButton.onClick.AddListener(OnSignUpClicked);
        _signupEnterButton.onClick.AddListener(OnSignUpEnterClicked);
        _loginButton.onClick.AddListener(OnLoginClicked);
        _logoutButton.onClick.AddListener(OnLogoutClicked);

        HideUI();
    }
    #endregion
}