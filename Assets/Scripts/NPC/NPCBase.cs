using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBase : Photon.MonoBehaviour {

    public Animator animator;
    public GameObject opponent;
    public float distance;

    void Start()
    {
        //opponent = PlayerNetwork.PlayerObject;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (PhotonNetwork.isMasterClient)
        {
            opponent = GetClosestOpponent();
            distance = Vector3.Distance(transform.position, opponent.transform.position);
            //if (distance < animator.GetFloat("Distance"))
            //{
                photonView.RPC("SetDistance", PhotonTargets.MasterClient);
            //}
        }
        //animator.SetFloat("Distance", Vector3.Distance(transform.position, PlayerNetwork.PlayerObject.transform.position));
    }

    GameObject GetClosestOpponent()
    {
        PlayerNameTag[] players = FindObjectsOfType<PlayerNameTag>();
        GameObject ClosestOpponent = players[0].gameObject;

        for (int i = 0; i < players.Length; i++)
        {
            if (i != 0)
            {
                if (Vector3.Distance(players[i - 1].gameObject.transform.position, gameObject.transform.position) > Vector3.Distance(players[i].gameObject.transform.position, gameObject.transform.position))
                {
                    ClosestOpponent = players[i].gameObject;
                }
            }
        }
        return ClosestOpponent;
    }

    [PunRPC]
    void SetDistance()
    {
        animator.SetFloat("Distance", Vector3.Distance(gameObject.transform.position, opponent.transform.position));
    }
}
