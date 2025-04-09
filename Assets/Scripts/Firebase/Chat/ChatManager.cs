using Firebase.Database;
using Firebase.Extensions;
using System.Collections.Generic;
using UnityEngine;

public class ChatManager : MonoBehaviour
{
    private DatabaseReference _dbReference;
    public string _nicname = "null nick";
    [SerializeField] private string _roomName = "room1";

    void Start()
    {
        _dbReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    // 메시지 전송
    public void SendMessage(string userId, string messageText)
    {
        // 새 메시지를 위한 고유 Key 생성
        string key = _dbReference.Child("chats").Child(_roomName).Push().Key;

        // 메시지 정보를 담을 Dictionary
        Dictionary<string, object> messageInfo = new Dictionary<string, object>();
        messageInfo["userId"] = userId;
        messageInfo["nickname"] = _nicname;     //TODO : 닉네임을 파이어베이스에서 가져오도록 해야함
        messageInfo["messageText"] = messageText;
        messageInfo["timestamp"] = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        // 파이어베이스에 업로드할 Dictionary
        Dictionary<string, object> childUpdates = new Dictionary<string, object>();
        childUpdates[$"chats/{_roomName}/{key}"] = messageInfo;

        // 업로드 실행
        _dbReference.UpdateChildrenAsync(childUpdates).ContinueWithOnMainThread(task => {
            if (task.IsCompleted)
            {
                Debug.Log("Message sent successfully.");
            }
            else
            {
                Debug.LogError("Failed to send message: " + task.Exception);
            }
        });
    }


}
