using UnityEngine;
using UnityEngine.UI;
using Firebase.Auth;
using TMPro;
using System.Collections;

public class AuthUIManager : MonoBehaviour
{
    #region Fields & Properties

    public TMP_InputField emailInputField;  // TMP_InputField 사용
    public TMP_InputField passwordInputField;  // TMP_InputField 사용
    public Button signUpButton;
    public Button loginButton;
    public Button logoutButton;
    public TextMeshProUGUI resultText;  // TextMeshProUGUI 사용

    [SerializeField]private GameObject failedUIObject;
    [SerializeField]private TextMeshProUGUI failedText;
    #endregion


    #region Custom Functions
    // 회원가입 클릭 시 처리
    void OnSignUpClicked()
    {
        string email = emailInputField.text;
        string password = passwordInputField.text;

        AuthManager.Instance.RegisterUser(email, password,
            (user) =>
            {
                resultText.text = "Sign Up Successful! ID: " + user.UserId;
            },
            (error) =>
            {
                resultText.text = "Sign Up Failed: " + error;
            });
    }

    // 로그인 클릭 시 처리
    void OnLoginClicked()
    {
        string email = emailInputField.text;
        string password = passwordInputField.text;

        AuthManager.Instance.LoginUser(email, password,
            (user) =>
            {
                Debug.Log("Login Successful! ID: " + user.UserId);
                resultText.text = "Login Successful! ID: " + user.UserId;
            },
            (error) =>
            {
                Debug.Log("Login Failed: 로그인 실패");
                LoginFalied();
            });
    }

    // 로그아웃 클릭 시 처리
    void OnLogoutClicked()
    {
        AuthManager.Instance.LogoutUser();
        resultText.text = "Logged Out!";
    }

    public void LoginFalied()
    {
        failedText.text = "Login Failed...";
        StartCoroutine(ShowFailedUI());
    }
    #endregion

    #region Coroutine Methods
    private IEnumerator ShowFailedUI()
    {
        Debug.Log("로그인 실패 코루틴");
        failedUIObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        failedUIObject.SetActive(false);
    }
    #endregion

    #region Unity Built-in Functions
    void Start()
    {
        signUpButton.onClick.AddListener(OnSignUpClicked);
        loginButton.onClick.AddListener(OnLoginClicked);
        logoutButton.onClick.AddListener(OnLogoutClicked);

        failedUIObject = transform.GetChild(6).gameObject;
        failedText = failedUIObject.GetComponentInChildren<TextMeshProUGUI>();
    }
    #endregion
}