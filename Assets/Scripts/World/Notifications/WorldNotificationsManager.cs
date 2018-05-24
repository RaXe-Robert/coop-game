using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using ExitGames.Client.Photon;
using System.IO;
using System;

/// <summary>
/// Responsible for showing, and keeping track off, world notifications.
/// </summary>
/// <remarks>World notifcations can be anything, ranging from damage indicators to markers.</remarks>
public class WorldNotificationsManager : Photon.MonoBehaviour
{
    public static WorldNotificationsManager Instance
    {
        get; private set;
    }

    [SerializeField] private GameObject worldNotificationPrefab;

    private List<WorldNotification> currentWorldNotifications = new List<WorldNotification>();
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    private void Update()
    {
        foreach (WorldNotification worldNotification in currentWorldNotifications)
        {
            //worldNotification.Tick();
        }
    }

    public void ShowNotification(WorldNotificationArgs worldNotificationArgs, bool includeLocal)
    {
        if (includeLocal)
            photonView.RPC("CreateNotification", PhotonTargets.All, worldNotificationArgs.Position, worldNotificationArgs.Text, worldNotificationArgs.Duration, worldNotificationArgs.Color);
        else
            photonView.RPC("CreateNotification", PhotonTargets.Others, worldNotificationArgs.Position, worldNotificationArgs.Text, worldNotificationArgs.Duration, worldNotificationArgs.Color);
    }

    public void ShowLocalNotification(WorldNotificationArgs worldNotificationArgs)
    {
        GameObject worldNotificationObj = Instantiate(worldNotificationPrefab);
        WorldNotification worldNotification = worldNotificationObj.GetComponent<WorldNotification>();
        worldNotification.InitializeAndStart(worldNotificationArgs);
    }

    [PunRPC]
    private void CreateNotification(Vector3 position, string text, float duration, string color)
    {
        GameObject worldNotificationObj = Instantiate(worldNotificationPrefab);
        WorldNotification worldNotification = worldNotificationObj.GetComponent<WorldNotification>();
        worldNotification.InitializeAndStart(new WorldNotificationArgs(position, text, duration, color));
    }
}
