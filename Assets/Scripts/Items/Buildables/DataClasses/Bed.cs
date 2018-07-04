using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Bed : BuildableWorldObject
{
    [SerializeField] GameObject sleepingCharacterPrefab;
    private bool occupied = false;
    private GameObject playerInBed;
    private GameObject objectInBed;

    protected override UnityAction[] InitializeActions()
    {
        return new UnityAction[]
        {
            new UnityAction(EnterBed),
            new UnityAction(LeaveBed)
        };
    }

    public override void Interact(GameObject invoker, Item item)
    {
        if (!InRange(invoker.transform.position))
            return;

        if (item?.GetType() == typeof(Tool))
        {
            Tool tool = item as Tool;
            if (tool.ToolType == ToolType.Hammer)
                Actions[0].Invoke();
        }
        else if (!occupied)
            Actions[1].Invoke();
        else if (playerInBed != null)
            if (playerInBed == PlayerNetwork.LocalPlayer)
                Actions[2].Invoke();

        if (occupied && playerInBed == null)
            FeedUI.Instance.AddFeedItem("Leave this person alone!", feedType: FeedItem.Type.Fail);            
    }

    private void Update()
    {
        if(playerInBed && DaytimeController.Instance.IsDaytime)
        {
            LeaveBed();
        } 
    }

    private void EnterBed()
    {
        if (!DaytimeController.Instance.IsDaytime)
        {
            playerInBed = PlayerNetwork.LocalPlayer;
            playerInBed.GetComponent<PlayerMovementController>().IsFrozen = true;
            playerInBed.GetComponent<PlayerCombatController>().TogglePlayerModel(false);
            photonView.RPC(nameof(RPC_BedFeed), PhotonTargets.All, PhotonNetwork.player.NickName);
            photonView.RPC(nameof(RPC_EnterBed), PhotonTargets.AllBuffered);

            ExitGames.Client.Photon.Hashtable inBed = new ExitGames.Client.Photon.Hashtable() { { "inBed", true } };
            PhotonNetwork.SetPlayerCustomProperties(inBed);

            if (AllPlayersInBed())
            {
                DaytimeController.Instance.CurrentTime = DaytimeController.Instance.CurrentTime + new System.TimeSpan(DaytimeController.Instance.NightDuration, 0, 0);
            }
        }
        else
            FeedUI.Instance.AddFeedItem("Its daytime you fool!", feedType: FeedItem.Type.Fail);
    }

    private bool AllPlayersInBed()
    {
        if (PhotonNetwork.otherPlayers.Length > 0)
        {
            foreach (PhotonPlayer p in PhotonNetwork.otherPlayers)
            {
                if ((bool)p.CustomProperties["inBed"] == false)
                {
                    return false;
                }
            }
        }
        
        return true;
    }

    private void LeaveBed()
    {
        playerInBed.GetComponent<PlayerMovementController>().IsFrozen = false;
        playerInBed.GetComponent<PlayerCombatController>().TogglePlayerModel(true);
        playerInBed = null;
        photonView.RPC(nameof(RPC_LeaveBed), PhotonTargets.AllBuffered);
        

        ExitGames.Client.Photon.Hashtable inBed = new ExitGames.Client.Photon.Hashtable() { { "inBed", false } };
        PhotonNetwork.SetPlayerCustomProperties(inBed);
    }

    [PunRPC]
    protected void RPC_BedFeed(string name)
    {
        FeedUI.Instance.AddFeedItem(name + " has gone to bed!", feedType: FeedItem.Type.World);
    }

    [PunRPC]
    protected void RPC_EnterBed()
    {
        objectInBed = Instantiate(sleepingCharacterPrefab, transform);
        occupied = true;
    }

    [PunRPC]
    protected void RPC_LeaveBed()
    {
        Destroy(objectInBed);
        occupied = false;
    }
}
