using UnityEngine;

public class DisconnectManager : MonoBehaviour
{
    public static DisconnectManager Instance;

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
    }


    private void OnDestroy()
    {
        LobbyAndSesssionFireBaseManager.Instance.RemoveSessionFromFirebase(LobbyAndSesssionUIManager.Instance.GetSavedJoinCode());
        Debug.Log("에디터 강제 종료");
    }
}
