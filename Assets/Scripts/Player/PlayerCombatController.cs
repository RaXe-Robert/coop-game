using System;
using UnityEngine;
using Photon;

public class PlayerCombatController : PunBehaviour, IAttackable, IAttacker
{
    public Vector3 Position => transform.position;
    public string TooltipText => photonView.owner.NickName;
    public float Damage => stats.Damage;

    private PlayerStatsComponent stats;
    private HealthComponent healthComponent;

    private void Awake()
    {
        stats = GetComponent<PlayerStatsComponent>();
        healthComponent = GetComponent<HealthComponent>();
    }

    public void TakeHit(IAttacker attacker)
    {
        healthComponent.DecreaseValue(attacker.Damage);
    }
}