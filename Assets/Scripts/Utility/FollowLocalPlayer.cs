using UnityEngine;
using System.Collections;

/// <summary>
/// Translates the attached transform component to the players position
/// </summary>
public class FollowLocalPlayer : MonoBehaviour
{
    private Transform playerTransform;
    
    [SerializeField]
    private Vector3 offset;

    [Tooltip("Ignore player transform values and use the offset value")]
    [SerializeField]
    private bool offsetOverrideX = false, offsetOverrideY = false, offsetOverrideZ = false;

    // Use this for initialization
    private void Start()
    {
        if (PlayerNetwork.LocalPlayer != null)
            playerTransform = PlayerNetwork.LocalPlayer.transform;

        PlayerNetwork.OnLocalPlayerSpawned += LocalPlayerSpawned;
    }

    private void OnDisable() => PlayerNetwork.OnLocalPlayerSpawned -= LocalPlayerSpawned;

    // Update is called once per frame
    private void LateUpdate()
    {
        if (playerTransform != null)
        {
            Vector3 pos = playerTransform.position;
            if (offsetOverrideX)
                pos.x = 0;
            if (offsetOverrideY)
                pos.y = 0;
            if (offsetOverrideZ)
                pos.z = 0;

            transform.position = pos + offset;
        }
    }

    private void LocalPlayerSpawned(GameObject localPlayerObject)
    {
        playerTransform = localPlayerObject.transform;
    }
}
