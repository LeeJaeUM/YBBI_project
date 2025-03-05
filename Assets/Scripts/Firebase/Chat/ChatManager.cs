using Firebase.Database;
using Firebase.Extensions;
using System.Collections.Generic;
using UnityEngine;

public class ChatManager : MonoBehaviour
{
    private DatabaseReference dbReference;
    private string roomName = "room1";

    void Start()
    {
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    // 메시지 전송
    public void SendMessage(string userId, string messageText)
    {
        // 새 메시지를 위한 고유 Key 생성
        string key = dbReference.Child("chats").Child(roomName).Push().Key;

        // 메시지 정보를 담을 Dictionary
        Dictionary<string, object> messageInfo = new Dictionary<string, object>();
        messageInfo["userId"] = userId;
        messageInfo["messageText"] = messageText;
        messageInfo["timestamp"] = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        // 파이어베이스에 업로드할 Dictionary
        Dictionary<string, object> childUpdates = new Dictionary<string, object>();
        childUpdates[$"chats/{roomName}/{key}"] = messageInfo;

        // 업로드 실행
        dbReference.UpdateChildrenAsync(childUpdates).ContinueWithOnMainThread(task => {
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
