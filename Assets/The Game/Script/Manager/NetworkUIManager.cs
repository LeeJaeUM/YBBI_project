using UnityEngine;

public class NetworkUIManager : MonoBehaviour
{
    [SerializeField] NetworkSkillCoolDownUI[] _networkSkillCoolDownUIs;
    [SerializeField] NetworkJoyStick _networkJoyStick;

    private void Awake()
    {
        _networkSkillCoolDownUIs = GetComponentsInChildren<NetworkSkillCoolDownUI>();
    }

    public void SetUpUIsInStart(PlayerATKStats playerAtkStats, TheGamePlayerInputHandler theGamePlayerInput)
    {
        for (int i = 0; i < _networkSkillCoolDownUIs.Length; i++)
        {
            _networkSkillCoolDownUIs[i].SetUpStart(playerAtkStats, theGamePlayerInput);
        }
    }
}