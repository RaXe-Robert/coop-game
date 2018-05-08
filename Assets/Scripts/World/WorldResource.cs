using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldResource : Photon.MonoBehaviour, IInteractable
{
    public new string name;
    public ToolType requiredToolToHarvest;
    public float interactDistance = 5f;
    [SerializeField] private GameObject spawnOnDepleted;
    [SerializeField] private HealthComponent healthComponent;
    [SerializeField] private ItemsToDropComponent itemsToDropComponent;
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!photonView.isMine)
            return;

        if (healthComponent.IsDepleted())
        {
            StartCoroutine(PlayDepletedAnimation());
        }
    }

    private IEnumerator PlayDepletedAnimation()
    {
        if (animator != null)
        {
            photonView.RPC("CallAnimation", PhotonTargets.All);
            yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0).Length + 1f);
        }

        itemsToDropComponent?.SpawnItemsOnDepleted();

        photonView.RPC("DestroyObject", PhotonTargets.MasterClient);
    }

    #region IInteractable Implementation

    public bool IsInteractable => true;
    public GameObject GameObject => gameObject;

    public bool InRange(Vector3 invokerPosition) =>
        Vector3.Distance(invokerPosition, transform.position) < interactDistance;

    public void Interact(GameObject invoker)
    {
        if (!InRange(invoker.transform.position))
            return;

        var playerMovement = PlayerNetwork.PlayerObject.GetComponent<PlayerMovementController>();
        if (!playerMovement.CanInteract)
        {
            WorldNotificationsManager.Instance
                .ShowNotification(new WorldNotificationArgs(transform.position, "Not ready yet", 1), true);
            return;
        }
        
        var stats = PlayerNetwork.PlayerObject.GetComponent<StatsComponent>();
        playerMovement.AddInteractionTimeout(stats.TimeBetweenResourceHits);

        var equipmentManager = PlayerNetwork.PlayerObject.GetComponent<EquipmentManager>();
        if (!equipmentManager.HasToolEquipped(requiredToolToHarvest))
        {
            WorldNotificationsManager.Instance
                .ShowNotification(new WorldNotificationArgs(transform.position, "Not ready yet", 1), true);
            return;
        }

        healthComponent.DecreaseValue(50f);
    }

    public string TooltipText()
    {
        return $"{name} \nRequires {requiredToolToHarvest}";
    }

    #endregion //IInteractable Implementation

    [PunRPC]
    void CallAnimation()
    {
        animator.SetBool("isDepleted", true);
    }

    [PunRPC]
    void DestroyObject()
    {
        PhotonNetwork.Destroy(gameObject);
    }
}
