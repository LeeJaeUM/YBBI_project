using UnityEngine;
using Unity.Netcode;

public class PlayerSetupManager : NetworkBehaviour
{
    SkillCoolDownUIManager skillCoolDownUI;
    PlayerATKStats playerATKStats;

    void Start()
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
        skillCoolDownUI = FindObjectOfType<SkillCoolDownUIManager>();
        if (skillCoolDownUI == null)
        {
            Debug.LogError("씬에서 SkillCoolDownUI를 찾을 수 없음!");
            return;
        }

        // SkillCoolDownUI 세팅
        skillCoolDownUI.SetUpStart(playerATKStats);
    }
}