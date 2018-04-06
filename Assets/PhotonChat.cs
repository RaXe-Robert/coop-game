using ExitGames.Client.Photon;
using ExitGames.Client.Photon.Chat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonChat : Photon.MonoBehaviour, IChatClientListener {

    private ChatClient chatClient;

    void Start () {
        chatClient = new ChatClient(this);
        chatClient.Connect("54532ce7-d2de-4b74-85e8-645c60479f4e", "1", null);
	}
	
	// Update is called once per frame
	void Update () {
        chatClient.Service();
    }

    public void DebugReturn(DebugLevel level, string message)
    {
        Debug.Log(level + " " + message);
    }

    public void OnChatStateChange(ChatState state)
    {
        throw new System.NotImplementedException();
    }

    public void OnConnected()
    {
        throw new System.NotImplementedException();
    }

    public void OnDisconnected()
    {
        throw new System.NotImplementedException();
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        string msgs = "";
        for (int i = 0; i < senders.Length; i++)
        {
            msgs = string.Format("{0}{1}={2}, ", msgs, senders[i], messages[i]);
        }
        Debug.Log("OnGetMessages: " + channelName + "(" + senders.Length + ") >" + msgs);
        // All public messages are automatically cached in `Dictionary<string, ChatChannel> PublicChannels`. 
        // So you don't have to keep track of them. 
        // The channel name is the key for `PublicChannels`.
        // In very long or active conversations, you might want to trim each channels history.
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        Debug.Log("OnPrivateMessage: "+ channelName + "(" + sender + ") > "+ message);
        // All private messages are automatically cached in `ChatClient.PrivateChannels`, so you don't have to keep track of them. 
        // A channel name is applied as key for `PrivateChannels`. 
        // Get a (remote) user's channel name with `ChatClient.GetPrivateChannelNameByUser(name)`.
        // e.g. To get and show all messages of a private channel:
        // ChatChannel ch = this.chatClient.PrivateChannels[ channelName ];
        // foreach ( object msg in ch.Messages )
        // {
        //     Console.WriteLine( msg );
        // }
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        throw new System.NotImplementedException();
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        throw new System.NotImplementedException();
    }

    public void OnUnsubscribed(string[] channels)
    {
        throw new System.NotImplementedException();
    }

    
}
