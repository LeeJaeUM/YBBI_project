//TODO : 2025.02.25 데이터베이스 작업 시 주석 해제해야함
/*
using Firebase;
using Firebase.Extensions;
using Firebase.Firestore;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class FirebaseDatabase : MonoBehaviour
{
    private FirebaseFirestore db;


    // 데이터 가져오는 함수
    private async System.Threading.Tasks.Task LoadDataFromFirestore()
    {
        // "project01" 컬렉션 참조
        CollectionReference projectRef = db.Collection("project01");

        try
        {
            // "project01" 컬렉션의 모든 문서 가져오기
            QuerySnapshot snapshot = await projectRef.GetSnapshotAsync();

            // 각 문서에서 데이터를 추출하여 로그 출력
            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                // 문서 데이터를 딕셔너리로 변환
                Dictionary<string, object> documentDictionary = document.ToDictionary();

                // "id" 필드가 존재하는지 확인하고 출력
                if (documentDictionary.ContainsKey("id") && documentDictionary["id"] != null)
                {
                    Debug.Log("id: " + documentDictionary["id"] as string);
                }
                else
                {
                    Debug.LogWarning("id 필드가 없습니다.");
                }

                // "nickname" 필드가 존재하는지 확인하고 출력
                if (documentDictionary.ContainsKey("nickname") && documentDictionary["nickname"] != null)
                {
                    Debug.Log("nickname: " + documentDictionary["nickname"] as string);
                }
                else
                {
                    Debug.LogWarning("nickname 필드가 없습니다.");
                }

                // "password" 필드가 존재하는지 확인하고 출력
                if (documentDictionary.ContainsKey("password") && documentDictionary["password"] != null)
                {
                    Debug.Log("password: " + documentDictionary["password"] as string);
                }
                else
                {
                    Debug.LogWarning("password 필드가 없습니다.");
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Firestore에서 데이터를 가져오는 중 오류가 발생했습니다: " + ex.Message);
        }
    }

    // 데이터 업로드 함수
    // 유저의 데이터를 Firestore에 업로드하는 함수
    public async Task UploadDataToFirestore(string userId, string nickname, string password, int level, int score, int jobType, int coins)
    {
        // Firestore 인스턴스 가져오기
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;

        // 유저의 데이터를 저장할 문서 참조 (userId를 문서 ID로 사용)
        DocumentReference userRef = db.Collection("users").Document(userId);

        // 유저의 기본 데이터 (nickname, password 등)
        Dictionary<string, object> userData = new Dictionary<string, object>
        {
            { "nickname", nickname },
            { "password", password }, // 비밀번호는 반드시 암호화 후 저장해야 합니다!
            { "createdAt", FieldValue.ServerTimestamp },
            { "lastLogin", FieldValue.ServerTimestamp },
        };

        // 유저 데이터 업로드 (유저 문서에 저장)
        await userRef.SetAsync(userData);

        // 게임 진행 데이터 (gameData 하위 컬렉션에 저장)
        CollectionReference gameDataRef = userRef.Collection("gameData");
        DocumentReference gameProgressRef = gameDataRef.Document("currentProgress");

        // 게임 진행 상황 데이터
        Dictionary<string, object> gameData = new Dictionary<string, object>
        {
            { "level", level },
            { "score", score },
            { "jobType", jobType }, // 추가된 직업 타입
            { "coins", coins },
            { "lastCheckpoint", "checkpoint1" }, // 초기 체크포인트 (예시)
            { "playTime", 0 }, // 초기 플레이 시간 (예시)
        };

        // 게임 진행 데이터 업로드 (gameData 하위 컬렉션에 저장)
        await gameProgressRef.SetAsync(gameData);

        Debug.Log("유저 데이터와 게임 진행 데이터가 Firestore에 업로드되었습니다.");
    }

    #region Unity Built-in Functions

    // Start is called before the first frame update
    async void Start()
    {
        // FirebaseFirestore 인스턴스 가져오기
        db = FirebaseFirestore.DefaultInstance;

        // 데이터 가져오는 함수 호출
        await LoadDataFromFirestore();

        // 예시 유저 데이터
        string userId = "user123";  // 유저 ID (고유한 값)
        string nickname = "playerOne"; // 유저 닉네임
        string password = "password123"; // 유저 비밀번호 (실제 비밀번호는 암호화해서 저장해야 함)
        int level = 5; // 유저 레벨
        int score = 12000; // 유저 점수
        int jobType = 1; // 직업 타입 (예: 1은 전사, 2는 마법사 등)
        int coins = 150; // 유저가 가진 코인


        // 예시 데이터 업로드 함수 호출
        await UploadDataToFirestore(userId, nickname, password, level, score, jobType, coins);
    }
    #endregion
}

*/