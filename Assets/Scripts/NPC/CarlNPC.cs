using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarlNPC : MonoBehaviour {

    private Animator animator;
        
	// Use this for initialization
	void Start () {
        animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        animator.SetFloat("Distance", Vector3.Distance(transform.position, PlayerNetwork.PlayerObject.transform.position));	
	}
}
