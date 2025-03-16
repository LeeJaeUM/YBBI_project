using Firebase.Auth;
using Firebase;
using System.Threading.Tasks;
using UnityEngine;

public class FirebaseAuthManager : MonoBehaviour
{
    private FirebaseAuth auth;
    private FirebaseUser user;

    private async void Start()
    {
        await InitializeFirebaseAuth();
    }

    private async Task InitializeFirebaseAuth()
    {
        // 🔥 Firebase 인증 초기화
        auth = FirebaseAuth.DefaultInstance;

        if (auth.CurrentUser == null)
        {
            Debug.Log("Firebase 익명 로그인 시도 중...");
            await SignInAnonymously();
        }
        else
        {
            user = auth.CurrentUser;
            Debug.Log($"Firebase 로그인 완료: {user.UserId}");
        }
    }

    private async Task SignInAnonymously()
    {
        try
        {
            var authResult = await auth.SignInAnonymouslyAsync(); // 🔥 최신 버전 적용
            user = authResult.User; // 🔥 변경된 부분 (AuthResult에서 User 가져오기)

            Debug.Log($"Firebase 익명 로그인 성공! 사용자 ID: {user.UserId}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Firebase 로그인 실패: {e.Message}");
        }
    }
}
