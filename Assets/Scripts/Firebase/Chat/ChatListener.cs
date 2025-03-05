using Firebase.Database;
using Firebase.Extensions;
using System;
using UnityEngine;

public class ChatListener : MonoBehaviour
{
    private DatabaseReference _chatRef;
    private string _roomName = "room1";
    public ChatUIController _chatUIController;

    private void HandleNewMessage(object sender, ChildChangedEventArgs e)
    {
        if (e.Snapshot == null || e.Snapshot.Value == null) return;

        string key = e.Snapshot.Key;
        string userId = e.Snapshot.Child("userId").Value.ToString();
        string messageText = e.Snapshot.Child("messageText").Value.ToString();
        string timestamp = e.Snapshot.Child("timestamp").Value.ToString();

#if UNITY_EDITOR
        Debug.Log($"[New] {userId}: {messageText} (timestamp: {timestamp})");
#endif
        _chatUIController.DisplayNewMessage(userId, messageText, timestamp);
    }

    void Start()
    {
        _chatRef = FirebaseDatabase.DefaultInstance
            .GetReference("chats")
            .Child(_roomName);

        _chatRef.ChildAdded += HandleNewMessage;     //새로운 데이터가 생길때 새로운 데이터만 받아옴 (이전의 데이터는 없음)
    }

    void OnDestroy()
    {
        if (_chatRef != null)
        {
            try
            {
                _chatRef.ChildAdded -= HandleNewMessage;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Failed to remove event listener: {e.Message}");
            }
        }
    }
}
//private void HandleChatValueChanged(object sender, ValueChangedEventArgs e)
//{
//    if (e.Snapshot == null || e.Snapshot.Value == null)
//    {
//        Debug.Log("No chat data.");
//        return;
//    }

//    // 스냅샷에서 전체 채팅 데이터를 받아옴
//    foreach (var childSnapshot in e.Snapshot.Children)
//    {
//        string key = childSnapshot.Key;
//        string userId = childSnapshot.Child("userId").Value?.ToString();
//        string messageText = childSnapshot.Child("messageText").Value?.ToString();
//        string timestamp = childSnapshot.Child("timestamp").Value?.ToString();

//        // UI 등에 메시지를 표시하거나 로직 처리
//        Debug.Log($"[{key}] {userId}: {messageText} (timestamp: {timestamp})");
//    }
//}


//void Start()
//{
//    chatRef = FirebaseDatabase.DefaultInstance
//        .GetReference("chats")
//        .Child(roomName);

//    // 데이터에 변화가 있을 때마다 콜백
//    chatRef.ValueChanged += HandleChatValueChanged;
//}

