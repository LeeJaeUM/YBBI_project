using UnityEngine;

public class StartManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (LobbyAndSesssionUIManager.Instance != null)
        {
            LobbyAndSesssionUIManager.Instance.HideAllUi();
        }
    }
}
