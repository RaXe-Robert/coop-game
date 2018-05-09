using System;
using UnityEngine;
using Photon;

public class PlayerCombatController : PunBehaviour, IAttackable, IAttacker
{
    public string TooltipText => photonView.owner.NickName;
    public Vector3 Position => transform.position;
    public float Damage => stats.Damage;
    public bool IsDead { get; private set; }

    private PlayerStatsComponent stats;
    private HealthComponent healthComponent;

    public float TimeBetweenAttacks => stats.TimeBetweenAttacks;

    private void Awake()
    {
        stats = GetComponent<PlayerStatsComponent>();
        healthComponent = GetComponent<HealthComponent>();
        healthComponent.OnDepletedCallback += Die;
    }

    private void Die()
    {
        IsDead = true;
        Debug.Log("Played died");
        GameInterfaceManager.Instance.ToggleGameInterface(GameInterface.DeathScreen);
    }

    public void TakeHit(IAttacker attacker)
    {
        healthComponent.DecreaseValue(attacker.Damage);
    }
}