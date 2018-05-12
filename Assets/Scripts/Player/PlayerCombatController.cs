using System;
using UnityEngine;
using Photon;

public class PlayerCombatController : PunBehaviour, IAttackable, IAttacker
{
    public string Name => photonView.owner.NickName;
    public string TooltipText => Name;
    public GameObject GameObject => gameObject;
    public float Damage => stats.Damage;
    public bool IsDead { get; set; }
    private IAttacker lastAttacker;

    private PlayerStatsComponent stats;
    private HealthComponent healthComponent;

    public float TimeBetweenAttacks => stats.TimeBetweenAttacks;

    private void Awake()
    {
        stats = GetComponent<PlayerStatsComponent>();
        healthComponent = GetComponent<HealthComponent>();

        if(photonView.isMine)
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
            CustomInRoomChat.Instance.AddLine($"{Name} got killed by {lastAttacker.Name}");
        }
    }

    public void TakeHit(IAttacker attacker)
    {
        if (photonView.isMine)
        {
            lastAttacker = attacker;
            healthComponent.DecreaseValue(attacker.Damage - stats.Defense);
        }
    }
}