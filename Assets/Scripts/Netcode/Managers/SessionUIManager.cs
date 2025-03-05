using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using static RelayManager;

public class SessionUIManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField sessionNameInput;
    [SerializeField] private Toggle privateToggle;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private Transform sessionListContainer;
    [SerializeField] private GameObject sessionListItemPrefab;

    public async void CreateSession()
    {
        string sessionName = sessionNameInput.text;
        bool isPrivate = privateToggle.isOn;
        string password = passwordInput.text;

        await RelayManager.Instance.CreateRelay(sessionName, isPrivate, password);
        RefreshSessionList();
    }

    public void RefreshSessionList()
    {
        foreach (Transform child in sessionListContainer)
        {
            Destroy(child.gameObject);
        }

        List<SessionData> sessions = RelayManager.Instance.GetSessionList();
        foreach (var session in sessions)
        {
            GameObject sessionItem = Instantiate(sessionListItemPrefab, sessionListContainer);
            sessionItem.GetComponentInChildren<TMP_Text>().text = $"{session.SessionName} ({(session.IsPrivate ? "🔒" : "🆓")})";

            Button joinButton = sessionItem.GetComponentInChildren<Button>();
            joinButton.onClick.AddListener(() => JoinSession(session.JoinCode));
        }
    }

    public async void JoinSession(string joinCode)
    {
        string password = passwordInput.text;
        bool success = await RelayManager.Instance.JoinRelay(joinCode, password);
        if (!success)
        {
            Debug.LogError("세션 참가 실패!");
        }
    }
}
