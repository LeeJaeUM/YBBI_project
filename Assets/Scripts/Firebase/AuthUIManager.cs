using UnityEngine;
using UnityEngine.UI;
using Firebase.Auth;
using TMPro;

public class AuthUIManager : MonoBehaviour
{
    public TMP_InputField emailInputField;  // TMP_InputField 사용
    public TMP_InputField passwordInputField;  // TMP_InputField 사용
    public Button signUpButton;
    public Button loginButton;
    public Button logoutButton;
    public TextMeshProUGUI resultText;  // TextMeshProUGUI 사용

    void Start()
    {
        signUpButton.onClick.AddListener(OnSignUpClicked);
        loginButton.onClick.AddListener(OnLoginClicked);
        logoutButton.onClick.AddListener(OnLogoutClicked);
    }

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
                resultText.text = "Login Successful! ID: " + user.UserId;
            },
            (error) =>
            {
                resultText.text = "Login Failed: " + error;
            });
    }

    // 로그아웃 클릭 시 처리
    void OnLogoutClicked()
    {
        AuthManager.Instance.LogoutUser();
        resultText.text = "Logged Out!";
    }
}