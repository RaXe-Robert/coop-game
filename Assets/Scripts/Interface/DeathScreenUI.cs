using System;
using UnityEngine;
using UnityEngine.UI;

public class DeathScreenUI : MonoBehaviour
{
    public void Click_Respawn()
    {
        PlayerNetwork.RespawnPlayer();
        PlayerNetwork.PlayerObject.GetComponent<PlayerMovementController>().enabled = true;
        PlayerNetwork.PlayerObject.GetComponent<PlayerCombatController>().IsDead = false;
        GameInterfaceManager.Instance.CloseAllInterfaces();
    }
}
