using System;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Netcode;
using UnityEngine;

public class TestNet_TeamPlayer : NetworkBehaviour
{
    //[SerializeField] private Renderer _teamColorRenderer;
    [SerializeField] private SpriteRenderer _teamColorRenderer;
    [SerializeField] private Color[] _teamColors;

    //네트워크 바이트  변수
    [SerializeField]private NetworkVariable<byte> _teamIndex = new NetworkVariable<byte>(byte.MaxValue);

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        ChangeColor(_teamIndex.Value);
    }

    [ServerRpc]
    public void SetTeamServerRpc(byte newTeamIndex)
    {
        if (newTeamIndex > 3) { return; }

        _teamIndex.Value = newTeamIndex;
    }

    private void OnEnable()
    {
        _teamIndex.OnValueChanged += OnTeamChanged;
    }

    private void OnDisable()
    {
        _teamIndex.OnValueChanged -= OnTeamChanged;
    }

    private void OnTeamChanged(byte previousValue, byte newValue)
    {
        if(!IsClient) { return; }

        // _teamColorRenderer.material.SetColor("_BaseColor", _teamColors[newValue]);
        // _teamColorRenderer.color = _teamColors[newValue]; // SpriteRenderer에서 색상 변경 인데 배열의 color를 못받아와서 아래처럼 임시 변경

        ChangeColor(newValue);
    }

    /// <summary>
    /// 임시 색상 변경용 함수
    /// </summary>
    private void ChangeColor(byte newTeamInput)
    {
        switch (newTeamInput)
        {
            case 0:
                _teamColorRenderer.color = Color.blue;
                break;
            case 1:
                _teamColorRenderer.color = Color.red;
                break;
            case 2:
                _teamColorRenderer.color = Color.green;
                break;
            case 3:
                _teamColorRenderer.color = Color.magenta;
                break;
        }
    }
}
