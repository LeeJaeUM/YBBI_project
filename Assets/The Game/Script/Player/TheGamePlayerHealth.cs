using UnityEngine;
using Unity.Netcode;

public class TheGamePlayerHealth : UnitHealth
{
    [SerializeField] private TheGamePlayerAnimator _playerAnimator;
protected override void HitDamage()
{
    _playerAnimator.PlayHitAnimation();
}

protected override void Awake()
{
    base.Awake();
    _playerAnimator = GetComponent<TheGamePlayerAnimator>();
}
}