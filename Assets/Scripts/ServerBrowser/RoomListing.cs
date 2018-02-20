using UnityEngine;
using UnityEngine.UI;

using System.Collections;

[RequireComponent(typeof(Button))]
public class RoomListing : MonoBehaviour
{
    [SerializeField] private Text _roomNameDisplayText;
    public string RoomNameText
    {
        get { return _roomNameDisplayText.text; }
    }

    public string RoomName { get; private set; }
    public bool Updated { get; set; }

    #region MonoBehaviour

    private void Start()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    #endregion //MonoBehaviour

    #region Private Methods

    private void OnClick()
    {
        if (PhotonNetwork.JoinRoom(RoomName))
        {
            Debug.Log($"Succesfully joined {RoomName}");
        }
        else
        {
            Debug.LogWarning($"Failed to join {RoomName}");
        }
    }

    #endregion //Private Methods

    #region Public Methods

    public void SetRoomNameText(string text)
    {
        RoomName = text;
        _roomNameDisplayText.text = RoomName;
    }

    #endregion //Public Methods
}
