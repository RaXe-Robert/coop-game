using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Bed : BuildableWorldObject
{
    private bool occupied = false;
    private GameObject playerInBed;

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
        else if (occupied == false)
            Actions[1].Invoke();
        else if (playerInBed != null)
            if(playerInBed == PlayerNetwork.LocalPlayer)
                Actions[2].Invoke();
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
            photonView.RPC(nameof(RPC_EnterBed), PhotonTargets.AllBuffered, PhotonNetwork.player.NickName);

            ExitGames.Client.Photon.Hashtable inBed = new ExitGames.Client.Photon.Hashtable() { { "inBed", true } };
            PhotonNetwork.SetPlayerCustomProperties(inBed);

            if (AllPlayersInBed())
            {
                //TimeToNextDay is not the right timespan atm.
                System.TimeSpan TimeToNextDay = System.TimeSpan.FromTicks(DaytimeController.Instance.CurrentTime.Ticks % new System.TimeSpan(9, 0, 0).Ticks);
                DaytimeController.Instance.CurrentTime = DaytimeController.Instance.CurrentTime + TimeToNextDay;
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
        playerInBed = null;
        photonView.RPC(nameof(RPC_LeaveBed), PhotonTargets.AllBuffered);

        ExitGames.Client.Photon.Hashtable inBed = new ExitGames.Client.Photon.Hashtable() { { "inBed", false } };
        PhotonNetwork.SetPlayerCustomProperties(inBed);
    }

    [PunRPC]
    protected void RPC_EnterBed(string name)
    {
        FeedUI.Instance.AddFeedItem(name + " has gone to bed!", feedType: FeedItem.Type.World);
        occupied = true;
    }

    [PunRPC]
    protected void RPC_LeaveBed()
    {
        occupied = false;
    }
}
