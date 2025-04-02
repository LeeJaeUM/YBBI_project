using UnityEngine;

public class NetworkUIManager : MonoBehaviour
{
    [SerializeField] NetworkSkillCoolDownUI[] networkSkillCoolDownUIs;

    private void Awake()
    {
        networkSkillCoolDownUIs = GetComponentsInChildren<NetworkSkillCoolDownUI>();
    }

    public void SetUpUIsInStart(PlayerATKStats playerAtkStats, TheGamePlayerInputHandler theGamePlayerInput)
    {
        for (int i = 0; i < networkSkillCoolDownUIs.Length; i++)
        {
            networkSkillCoolDownUIs[i].SetUpStart(playerAtkStats, theGamePlayerInput);
        }
    }
}