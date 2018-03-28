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
    [SerializeField] private ItemsToDropComponent itemsToDrop;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private IEnumerator PlayDepletedAnimation()
    {
        if (animator != null)
        {
            photonView.RPC("CallAnimation", PhotonTargets.All);
            yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0).Length + 1f);
        }
        
        if (spawnOnDepleted != null)
        {
            itemsToDrop.SpawnObjectOnParent(spawnOnDepleted);
        }

        itemsToDrop.SpawnItemsOnDepleted();

        photonView.RPC("DestroyObject", PhotonTargets.MasterClient);
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

        healthComponent.DecreaseValue(50f);

        if (healthComponent.IsDepleted())
        {
            StartCoroutine(PlayDepletedAnimation());
        }
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
