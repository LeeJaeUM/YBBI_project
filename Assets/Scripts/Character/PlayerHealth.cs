using System;
using UnityEngine;

public class PlayerHealth : UnitHealth
{
    //protected override void Die()
    //{
    //    OnDie?.Invoke();
    //}

    [SerializeField] private PlayerAnimator _playerAnimator;
    protected override void HitDamage()
    {
        if(_playerAnimator == null)
        {
            Debug.LogError("PlayerAnimator is not assigned.");
            return;
        }
        _playerAnimator.PlayHitAnimation();
    }

    protected override void Awake()
    {
        base.Awake();
        _playerAnimator = GetComponent<PlayerAnimator>();
    }
}
