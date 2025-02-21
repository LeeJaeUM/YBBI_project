using System;
using UnityEngine;
using Firebase;
using Firebase.Auth;

public class AuthManager : MonoBehaviour
{
    public static AuthManager Instance; // 싱글톤 패턴

    private FirebaseAuth auth;
    private FirebaseUser currentUser;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        auth = FirebaseAuth.DefaultInstance;
    }

    // 회원가입 (이메일 & 비밀번호)
    public void RegisterUser(string email, string password, Action<FirebaseUser> onSuccess, Action<string> onError)
    {
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                onError?.Invoke("회원가입 실패: " + task.Exception);
                return;
            }

            FirebaseUser newUser = task.Result.User;
            onSuccess?.Invoke(newUser);
        });
    }

    // 로그인 (이메일 & 비밀번호)
    public void LoginUser(string email, string password, Action<FirebaseUser> onSuccess, Action<string> onError)
    {
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                onError?.Invoke("로그인 실패: " + task.Exception);
                return;
            }

            currentUser = task.Result.User;
            onSuccess?.Invoke(currentUser);
        });
    }

    // 로그아웃
    public void LogoutUser()
    {
        auth.SignOut();
        currentUser = null;
        Debug.Log("로그아웃 완료");
    }

    // 현재 로그인한 사용자 가져오기
    public FirebaseUser GetCurrentUser()
    {
        return auth.CurrentUser;
    }
}
