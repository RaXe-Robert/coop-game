using System;
using UnityEngine;
using Photon;

public class PlayerCombatController : PunBehaviour, IAttackable, IAttacker
{
    [SerializeField] private GameObject rightHandBone;
    private GameObject spawnedHoldingObject;
    
    public string Name => photonView.owner.NickName;
    public string TooltipText => Name;
    public GameObject GameObject => gameObject;
    public float Damage => stats.Damage;
    public bool IsDead { get; set; }
    private IAttacker lastAttacker;

    private PlayerStatsComponent stats;
    private HealthComponent healthComponent;

    public float TimeBetweenAttacks => stats.TimeBetweenAttacks;

    public void TriggerHitAnimation()
    {
        var animator = GetComponent<Animator>();
        if (animator == null)
            return;
        
        animator.SetTrigger("Swing");
    }
    
    private void Awake()
    {
        stats = GetComponent<PlayerStatsComponent>();
        healthComponent = GetComponent<HealthComponent>();

        healthComponent.OnDepletedCallback += Die;
    }

    private void Die()
    {
        if (photonView.isMine)
        {
            GameInterfaceManager.Instance.ToggleGameInterface(GameInterface.DeathScreen);
            GetComponent<Inventory>().DropAllItems();
            GetComponent<PlayerMovementController>().enabled = false;
        }
        photonView.RPC(nameof(KillPlayer), PhotonTargets.All, Name, lastAttacker == null ? "hunger" : lastAttacker.Name);
    }

    public void TakeHit(IAttacker attacker)
    {
        lastAttacker = attacker;
        healthComponent.DecreaseValue(attacker.Damage - stats.Defense);
    }

    public void TogglePlayerModel(bool showModel)
    {
        photonView.RPC(nameof(RPC_TogglePlayerModel), PhotonTargets.AllBuffered, showModel);
    }

    public void RespawnPlayer()
    {
        Vector3 position = new Vector3(UnityEngine.Random.Range(-5f, 5f), 0.2f, UnityEngine.Random.Range(0.5f, 5f));
        transform.position = position;
        GetComponent<HealthComponent>().SetValue(100);
        GetComponent<HungerComponent>().SetValue(100);
        GetComponent<PhotonView>().RPC(nameof(RPC_RespawnPlayer), PhotonTargets.All);
    }

    public void SwitchHoldingItem(string itemId)
    {
        if (spawnedHoldingObject != null)
            Destroy(spawnedHoldingObject);
        
        var model = ItemFactory.GetModel(itemId);
        if (model == null) return;

        spawnedHoldingObject = Instantiate(model, rightHandBone.transform);
        
        if(photonView.isMine)
            photonView.RPC("RPC_SwitchItem", PhotonTargets.All, itemId);
    }

    [PunRPC]
    private void RPC_SwitchItem(string itemId)
    {
        if (!photonView.isMine)
            SwitchHoldingItem(itemId);
    }
    
    [PunRPC]
    protected void RPC_TogglePlayerModel(bool showModel)
    {
        foreach(var rend in GetComponentsInChildren<Renderer>())
        {
            rend.enabled = showModel;
        }
    }

    [PunRPC]
    private void RPC_RespawnPlayer()
    {
        IsDead = false;
        TogglePlayerModel(true);
    }

    [PunRPC]
    private void KillPlayer(string playerName, string killerName)
    {
        CustomInRoomChat.Instance.AddLine($"{playerName} got killed by {killerName}");
        IsDead = true;
        TogglePlayerModel(false);
    }
}