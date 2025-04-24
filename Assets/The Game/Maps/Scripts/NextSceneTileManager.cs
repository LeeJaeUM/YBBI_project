using UnityEngine;

public class NextSceneTileManager : MonoBehaviour
{
    [SerializeField] private GameObject mapOBJ;
    private MapData mapData;
    private NextSceneManager mapGridNextSceneManager;

    private void Awake()
    {
        mapData = mapOBJ.GetComponent<MapData>();
        mapGridNextSceneManager = mapOBJ.GetComponentInParent<NextSceneManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        Debug.Log("다음 씬으로 이동");
        mapGridNextSceneManager.RequestNextScene();
    }
}