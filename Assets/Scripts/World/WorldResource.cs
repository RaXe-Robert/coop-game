using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(ItemsToDropComponent))]
public class WorldResource : MonoBehaviour, IInteractable
{
    [SerializeField] private new string name;
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private ToolType requiredToolToHarvest;
    [SerializeField] private float interactDistance = 5f;
    [SerializeField] private GameObject spawnOnDepleted;

    private Animator animator;
    private ItemsToDropComponent itemsToDrop;

    public string Name => name;
    public float MaxHealth => maxHealth;
    public ToolType RequiredToolToHarvest => requiredToolToHarvest;
    public float InteractDistance => interactDistance;

    private void Start()
    {
        animator = GetComponent<Animator>();
        itemsToDrop = GetComponent<ItemsToDropComponent>();
    }
    
    private IEnumerator PlayDepletedAnimation()
    {
        if (animator != null)
        {
            //photonView.RPC("CallAnimation", PhotonTargets.All);
            yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0).Length + 1f);
        }

        itemsToDrop?.SpawnItemsOnDepleted();

       // photonView.RPC("DestroyObject", PhotonTargets.MasterClient);
    }

    #region IInteractable Implementation

    public bool IsInteractable()
    {
        return true;
    }

    public void Interact(Vector3 invokerPosition)
    {
        if (Vector3.Distance(transform.position, invokerPosition) > interactDistance)
            return;

        Debug.Log("interacting");
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
