using Unity.Multiplayer.Center.NetcodeForGameObjectsExample.DistributedAuthority;
using UnityEngine;

public class StartManager : MonoBehaviour
{
    /*public static StartManager Instance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // 중복 방지
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // 씬 전환 시 유지
    }
*/
    void Start()
    {
        if (LobbyAndSesssionUIManager.Instance != null)
        {
            LobbyAndSesssionUIManager.Instance.HideAllUi();
        }
    }
}
