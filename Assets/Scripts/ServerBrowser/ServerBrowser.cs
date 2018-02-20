using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ServerBrowser : MonoBehaviour
{
    [SerializeField] private GameObject layoutGroup;
    
    [SerializeField] private GameObject roomListingPrefab;
    public GameObject RoomListingPrefab
    {
        get { return roomListingPrefab; }
    }

    private List<RoomListing> roomListingButtons = new List<RoomListing>();
    public List<RoomListing> RoomListingButtons
    {
        get { return roomListingButtons; }
    }

    #region MonoBehaviour

    private void OnEnable()
    {
        RefreshServerList();
    }

    #endregion //MonoBehaviour

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
        int index = RoomListingButtons.FindIndex(x => x.RoomName == room.Name);

        if (index == -1)
        {
            if (room.IsVisible && room.PlayerCount < room.MaxPlayers)
            {
                GameObject roomListingObj = Instantiate(RoomListingPrefab, layoutGroup.transform, false);

                RoomListing roomListing = roomListingObj.GetComponent<RoomListing>();
                RoomListingButtons.Add(roomListing);

                index = (RoomListingButtons.Count - 1);
            }
        }

        if (index != -1)
        {
            RoomListing roomListing = RoomListingButtons[index];
            roomListing.SetRoomNameText(room.Name);
            roomListing.Updated = true;
        }
    }

    private void RemoveOldRooms()
    {
        List<RoomListing> removeRooms = new List<RoomListing>();

        foreach (RoomListing roomListing in RoomListingButtons)
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
            RoomListingButtons.Remove(roomListing);
            Destroy(roomListingObj);
        }
    }

    #endregion //Private Methods
}
