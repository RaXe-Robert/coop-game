﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerPropertiesUI : Photon.MonoBehaviour {

    public GameObject playerItemPrefab;
    public GameObject players;
    bool contains;

    private Dictionary<PhotonPlayer, GameObject> currentPlayersWithObject;
    
    void Start () {
        currentPlayersWithObject = new Dictionary<PhotonPlayer, GameObject>();
    }

    void Update() {
        if (PhotonNetwork.otherPlayers.Length == 0 && currentPlayersWithObject.Count == 0)
            return;

        UpdateHealthAndHunger();
        if (currentPlayersWithObject.Count == PhotonNetwork.otherPlayers.Length)
            return;

        foreach (PhotonPlayer p in PhotonNetwork.otherPlayers)
        {
            if (!currentPlayersWithObject.ContainsKey(p))
                AddPlayer(p);
        }

        if (currentPlayersWithObject.Count != PhotonNetwork.otherPlayers.Length)
        {
            foreach (KeyValuePair<PhotonPlayer, GameObject> kvp in currentPlayersWithObject)
            {
                contains = false;
                foreach (PhotonPlayer p in PhotonNetwork.otherPlayers)
                {
                    if (kvp.Key == p)
                    {
                        contains = true;
                        break;
                    }
                }
                if(contains != true)
                {
                    RemovePlayer(kvp.Key);
                    break;
                }

            }
        }
    }

    private void UpdateHealthAndHunger()
    {
        foreach (KeyValuePair<PhotonPlayer, GameObject> kvp in currentPlayersWithObject)
        {
            Slider[] HealthAndHungerSliders = kvp.Value.GetComponentsInChildren<Slider>();
            foreach(Slider s in HealthAndHungerSliders)
            {
                if (s.name == "HealthSlider")
                {
                    if(kvp.Key.CustomProperties["Health"] != null)
                        s.value = (float)kvp.Key.CustomProperties["Health"];                    
                }
                else if(s.name == "HungerSlider")
                {
                    if (kvp.Key.CustomProperties["Hunger"] != null)
                        s.value = (float)kvp.Key.CustomProperties["Hunger"];
                }
            }
        }
    }

    /// <summary>
    /// Adds UI element for a photonplayer.
    /// </summary>
    /// <param name="player"></param>
    private void AddPlayer(PhotonPlayer player)
    {
        GameObject go = Instantiate(playerItemPrefab);
        go.transform.SetParent(players.transform);
        go.transform.localScale = Vector3.one;

        go.GetComponentInChildren<Text>().text = player.NickName;

        currentPlayersWithObject.Add(player, go);

        UpdateHealthAndHunger();
    }

    /// <summary>
    /// Removes the player UI object from the game and from the list.
    /// </summary>
    /// <param name="player"></param>
    private void RemovePlayer(PhotonPlayer player)
    {
        Destroy(currentPlayersWithObject[player]);
        currentPlayersWithObject.Remove(player);
    }
}
