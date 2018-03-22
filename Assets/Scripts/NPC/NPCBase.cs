﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBase : Photon.MonoBehaviour {

    public Animator Animator { get; private set; }
    public GameObject Opponent { get; private set; }
    public UnityEngine.AI.NavMeshAgent Agent { get; private set; }
    public float DistanceToOpponent { get; private set; }

    private void Awake()
    {
        Agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        Animator = GetComponent<Animator>();
        foreach (var x in Animator.GetBehaviours<NPCBaseFSM>())
        {
            x.npc = this;
        }
    }

    private void Update()
    {
        if (PhotonNetwork.isMasterClient)
        {
            Opponent = GetClosestOpponent();
            DistanceToOpponent = Vector3.Distance(transform.position, Opponent.transform.position);
            //if (distance < animator.GetFloat("Distance"))
            //{
                photonView.RPC("SetDistance", PhotonTargets.MasterClient);
            //}
        }
        //animator.SetFloat("Distance", Vector3.Distance(transform.position, PlayerNetwork.PlayerObject.transform.position));
    }

    private GameObject GetClosestOpponent()
    {
        PlayerNameTag[] players = FindObjectsOfType<PlayerNameTag>();
        GameObject closestOpponent = players[0].gameObject;

        for (int i = 0; i < players.Length; i++)
        {
            if (i != 0)
            {
                if (Vector3.Distance(players[i - 1].gameObject.transform.position, gameObject.transform.position) > Vector3.Distance(players[i].gameObject.transform.position, gameObject.transform.position))
                {
                    closestOpponent = players[i].gameObject;
                }
            }
        }
        return closestOpponent;
    }

    [PunRPC]
    void SetDistance()
    {
        Animator.SetFloat("Distance", Vector3.Distance(gameObject.transform.position, Opponent.transform.position));
    }
}
