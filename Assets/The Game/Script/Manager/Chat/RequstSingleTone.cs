using UnityEngine;

public class RequestSingleTone : MonoBehaviour
{
    public static RequestSingleTone Instance;

    TheGameChatListener _listener;
    TheGameChatUIController _controller;

    private void Awake()
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
     
        _listener = GetComponent<TheGameChatListener>();
        _controller = GetComponent<TheGameChatUIController>();
    }

    public void RequestClearChatContent()
    {
        _controller.ClearChatContent();
    }

    public void DestroySingleton()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
            Instance = null;
        }
    }

    public void RequestDestroyAllSingleTones()
    {
        DisconnectManager.Instance.DestroySingleton();
        LobbyAndSesssionFireBaseManager.Instance.DestroySingleton();
        LobbyAndSesssionUIManager.Instance.DestroySingleton();
        DestroySingleton();
    }

    public void RequestChatONOFF(bool isChatOn)
    {
        _controller.SetChatUIOnOff(isChatOn);
    }
    public void RequestNewChat()
    {
        _listener.RequestChat();
    }

}
