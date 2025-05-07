using UnityEngine;
using Unity.Netcode;
using System;

public class RPC_EnemyNetworkController : NetworkBehaviour
{
    private bool _isHost = false;
    private bool _isOwner = false;

    private NetworkVariable<bool> isFlipX = new NetworkVariable<bool>(false);
    private SpriteRenderer _spriteRenderer;


    public bool GetIsHost()
    {
        return _isHost;
    }
    public bool GetIsOwner()
    {
        return _isOwner;
    }

    public void SetFlipX(bool isFlip)
    {
        isFlipX.Value = isFlip; //스프라이트 반전
    }
    void Initialized()
    { 
        _isHost = NetworkManager.Singleton.IsHost;
        _isOwner = IsOwner;
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }
    private void OnFlipXChanged(bool previousValue, bool newValue)
    {
        if (!IsClient) return;

        _spriteRenderer.flipX = newValue; //스프라이트 반전
    }

    private void Awake()
    {
        Initialized();
    }

    private void OnEnable()
    {
        isFlipX.OnValueChanged += OnFlipXChanged;
    }


    private void OnDisable()
    {
        isFlipX.OnValueChanged -= OnFlipXChanged;
    }
}
