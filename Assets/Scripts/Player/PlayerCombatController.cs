using System;
using UnityEngine;
using Photon;

public class PlayerCombatController : PunBehaviour, IAttackable, IAttacker
{
    public string TooltipText => photonView.owner.NickName;
    public GameObject GameObject => gameObject;
    public float Damage => stats.Damage;
    public bool IsDead { get; set; }

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
        if(photonView.isMine)
        {
            GameInterfaceManager.Instance.ToggleGameInterface(GameInterface.DeathScreen);
            IsDead = true;
            GetComponent<Inventory>().DropAllItem();
            GetComponent<PlayerMovementController>().enabled = false;
        }
        Debug.Log("Played died");
    }

    public void TakeHit(IAttacker attacker)
    {
        healthComponent.DecreaseValue(attacker.Damage - stats.Defense);
    }
}