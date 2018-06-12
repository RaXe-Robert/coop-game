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

        Color32 playerColor = Color.red;
        switch (playerId % playerColors.Length + PhotonNetwork.room.PlayerCount)
        {
            case 1: playerColor = new Color32(255, 0, 0, 100); break;//red
            case 2: playerColor = new Color32(255, 127, 0, 100); break;//orange
            case 3: playerColor = new Color32(255, 255, 0, 100); break; //yellow
            case 4: playerColor = new Color32(0, 255, 0, 100); break;//green
            case 5: playerColor = new Color32(0, 0, 255, 100); break;//blue
            case 6: playerColor = new Color32(75, 0, 130, 100); break;//indigo
            case 7: playerColor = new Color32(143, 0, 255, 100); break;//purple

        }
        playerObject.GetComponentInChildren<SkinnedMeshRenderer>().materials[3].color = playerColor;
    }
}
