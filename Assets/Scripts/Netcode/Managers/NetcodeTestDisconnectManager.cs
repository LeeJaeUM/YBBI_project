using UnityEngine;

public class NetcodeTestDisconnectManager : MonoBehaviour
{
    public static NetcodeTestDisconnectManager Instance;

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
        NetcodeFireBaseManager.Instance.RemoveSessionFromFirebase(UIManager.Instance.GetSavedJoinCode());
        Debug.Log("에디터 강제 종료");
    }
}
