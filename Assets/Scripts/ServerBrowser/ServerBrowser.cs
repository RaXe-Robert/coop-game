using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ServerBrowser : MonoBehaviour
{
    [SerializeField] private GameObject layoutGroup;
    [SerializeField] private GameObject roomListingPrefab;

    private List<RoomListing> roomListingButtons = new List<RoomListing>();
    
    private void OnEnable()
    {
        RefreshServerList();
    }

    #region Photon Callbacks

    private void OnReceivedRoomListUpdate()
    {
        RefreshServerList();
    }

    #endregion //Photon Callbacks

    #region Private Methods

    private void RefreshServerList()
    {
        RoomInfo[] rooms = PhotonNetwork.GetRoomList();

        foreach (RoomInfo room in rooms)
        {
            RoomReceived(room);
        }

        RemoveOldRooms();
    }

    private void RoomReceived(RoomInfo room)
    {
        int index = roomListingButtons.FindIndex(x => x.RoomName == room.Name);

        if (index == -1)
        {
            if (room.IsVisible && room.PlayerCount < room.MaxPlayers)
            {
                GameObject roomListingObj = Instantiate(roomListingPrefab, layoutGroup.transform, false);

                RoomListing roomListing = roomListingObj.GetComponent<RoomListing>();
                roomListingButtons.Add(roomListing);

                index = (roomListingButtons.Count - 1);
            }
        }

        if (index != -1)
        {
            RoomListing roomListing = roomListingButtons[index];
            roomListing.SetRoomName(room.Name);
            roomListing.SetPlayerCount(room.PlayerCount, room.MaxPlayers);
            roomListing.Updated = true;
        }
    }

    private void RemoveOldRooms()
    {
        List<RoomListing> removeRooms = new List<RoomListing>();

        foreach (RoomListing roomListing in roomListingButtons)
        {
            if (!roomListing.Updated)
            {
                removeRooms.Add(roomListing);
            }
            else
            {
                roomListing.Updated = false;
            }
        }

        foreach (RoomListing roomListing in removeRooms)
        {
            GameObject roomListingObj = roomListing.gameObject;
            roomListingButtons.Remove(roomListing);
            Destroy(roomListingObj);
        }
    }

    #endregion //Private Methods
}
