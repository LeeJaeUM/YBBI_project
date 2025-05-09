using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TheGameChatUIController : MonoBehaviour
{
    public TMP_InputField inputField;  // 사용자 입력
    public Button sendButton;
    public Transform chatContent;      // 채팅 메시지를 배치할 부모 객체
    public GameObject messagePrefab;   // 채팅 메시지를 표시할 UI 프리팹
    public GameObject chatCanv;
    private TheGameChatManager chatManager;    // 채팅 로직 관리 스크립트
    private TheGameAuthManager authManager; // 사용자 정보 (userId) 관리


    void OnSendButtonClicked()
    {
        if (!string.IsNullOrEmpty(inputField.text))
        {
            string userId = authManager.GetCurrentUser()?.UserId; // 현재 로그인된 사용자의 ID 가져오기

            if (!string.IsNullOrEmpty(userId))
            {
                // Firebase에 메시지 전송
                chatManager.SendMessage(userId, inputField.text);

                // 입력창 초기화
                inputField.text = "";
            }
            else
            {
                Debug.LogError("사용자가 로그인되어 있지 않습니다.");
            }
        }
    }

    // 새 메시지가 들어왔을 때(ChildAdded 이벤트에서 호출)
    public void DisplayNewMessage(string nickname, string messageText, string timestamp)
    {
        // 프리팹 생성 후 UI에 표시
        GameObject newMessage = Instantiate(messagePrefab, chatContent);
        TMP_Text messageTextComponent = newMessage.GetComponentInChildren<TMP_Text>();
        messageTextComponent.text = $"{nickname}: {messageText}  | {timestamp}";
        // 필요한 경우 timestamp 등 추가 표시 가능
    }

    public void ClearChatContent()
    {
        foreach (Transform child in chatContent.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void SetChatUIOnOff(bool isChatOn)
    {
        chatCanv.SetActive(isChatOn);
    }

    private void Awake()
    {
        chatManager = GetComponent<TheGameChatManager>();
        authManager = TheGameAuthManager.Instance;
    }
    void Start()
    {
        sendButton.onClick.AddListener(OnSendButtonClicked);
        chatCanv.SetActive(false);
    }
}
