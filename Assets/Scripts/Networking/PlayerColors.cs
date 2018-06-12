using UnityEngine;

public class PlayerColors : MonoBehaviour
{
    [SerializeField]
    private Color32[] playerColors =
    {
        new Color32(255, 0, 0, 100),//red
        new Color32(255, 127, 0, 100),//orange
        new Color32(255, 255, 0, 100), //yellow
        new Color32(0, 255, 0, 100),//green
        new Color32(0, 0, 255, 100),//blue
        new Color32(75, 0, 130, 100),//indigo
        new Color32(143, 0, 255, 100)//purple
    };

    private void Start()
    {
        PlayerNetwork.OnLocalPlayerCreated += OnLocalPlayerCreated;

        PlayerNetwork.OnOtherPlayerCreated += OnOtherPlayerCreated;
    }

    private void OnLocalPlayerCreated(GameObject localPlayer)
    {
        foreach (var player in PlayerNetwork.OtherPlayers)
            ApplyPlayerColor(player.Value.GameObject, player.Value.PhotonPlayer.ID);

        ApplyPlayerColor(localPlayer, PhotonNetwork.player.ID);
    }

    private void OnOtherPlayerCreated(PhotonView otherPlayer)
    {
        ApplyPlayerColor(otherPlayer.gameObject, otherPlayer.ownerId);
    }

    private void ApplyPlayerColor(GameObject playerObject, int playerId)
    {
        if (playerObject == null)
            return;

        if (playerColors == null || playerColors.Length == 0)
            return;
        
        playerObject.GetComponentInChildren<SkinnedMeshRenderer>().materials[3].color = playerColors[playerId % (playerColors.Length - 1)];
    }
}
