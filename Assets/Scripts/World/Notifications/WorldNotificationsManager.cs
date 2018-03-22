using System.Collections;
using System.Collections.Generic;

using UnityEngine;

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

    private List<WorldNotification> currentWorldNotifactions = new List<WorldNotification>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    private void Update()
    {
        foreach (WorldNotification worldNotification in currentWorldNotifactions)
        {
            //worldNotification.Tick();
        }
    }

    public void NewNotification(Vector3 position, string text, float duration)
    {
        photonView.RPC("CreateWorldNotification", PhotonTargets.All, position, text, duration);
    }

    [PunRPC]
    private void CreateWorldNotification(Vector3 position, string text, float duration)
    {
        GameObject worldNotificationObj = Instantiate(worldNotificationPrefab);
        WorldNotification worldNotification = worldNotificationObj.GetComponent<WorldNotification>();
        worldNotification.InitializeAndStart(position, text, duration);
    }
}
