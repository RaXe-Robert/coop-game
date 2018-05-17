using System;
using UnityEngine;
using UnityEngine.UI;

public class DeathScreenUI : MonoBehaviour
{
    public void Click_Respawn()
    {
        PlayerNetwork.LocalPlayer.GetComponent<PlayerMovementController>().enabled = true;
        PlayerNetwork.LocalPlayer.GetComponent<PlayerCombatController>().RespawnPlayer();
        GameInterfaceManager.Instance.CloseAllInterfaces();
    }
}
