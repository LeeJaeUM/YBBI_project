using UnityEngine;

public class DontDestroyDatas : MonoBehaviour
{
    public static DontDestroyDatas Instance;

    public int money = 0;





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

    
}
