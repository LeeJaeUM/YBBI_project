using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextSceneManager : MonoBehaviour
{
    [SerializeField] private string nextSceneName;
    [SerializeField] private MapRandomSpawner mapRandomSpawner;
    [SerializeField] private MapRpc mapRpc;
    public void RequestNextScene()
    {
        if (string.IsNullOrEmpty(nextSceneName))
        {
            Debug.LogError("[NextSceneTeleport] 전환할 씬 이름이 비어 있습니다.");
            return;
        }

        Debug.Log($"[NextSceneTeleport] {nextSceneName} 씬으로 이동 요청");

        if(NetworkManager.Singleton.IsServer)
        {
            mapRandomSpawner.DespawnAllMaps();
        }
        else
        {
            mapRpc.DespawnAllMapsServerRpc();
        }
        
        NetworkManager.Singleton.SceneManager.LoadScene(nextSceneName, LoadSceneMode.Single);
    }

}
