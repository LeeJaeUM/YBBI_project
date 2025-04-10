using UnityEngine;
using Unity.Netcode;
using UnityEngine.Lumin;

public class PlayerSetupManager : NetworkBehaviour
{
    [SerializeField] NetworkUIManager networkUIManager;
    [SerializeField] PlayerATKStats playerATKStats;
    [SerializeField] TheGamePlayerInputHandler theGamePlayerInputHandler;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        SetUp();
    }

    void Start()
    {
       // SetUp();
    }

    private void SetUp()
    {
        if (!IsOwner) return;

        // 자신의 PlayerATKStats 가져오기
        playerATKStats = GetComponent<PlayerATKStats>();
        if (playerATKStats == null)
        {
            Debug.LogError("PlayerATKStats가 PlayerSetupManager에 없음!");
            return;
        }

        // 씬에 존재하는 SkillCoolDownUI 찾기
        networkUIManager = FindObjectOfType<NetworkUIManager>();
        
        if (networkUIManager == null)
        {
            Debug.LogError("씬에서 SkillCoolDownUI를 찾을 수 없음!");
            return;
        }

        theGamePlayerInputHandler = GetComponent<TheGamePlayerInputHandler>();

        // SkillCoolDownUI 세팅
        networkUIManager.SetUpUIsInStart(playerATKStats, theGamePlayerInputHandler);
    }
}