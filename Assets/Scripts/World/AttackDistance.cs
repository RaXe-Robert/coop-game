using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackDistance : MonoBehaviour {

    [SerializeField] private GameObject objectToAttack;
    [SerializeField] private ObjectHealth health;
    //[SerializeField] private Collider radius;
    private bool clickable = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider other)
    {        
       if (other.gameObject.name.Substring(0, 6) == "Player")
       {
           clickable = true;
       }
    }

    void OnMouseDown()
    {
        if (!clickable)
        {
            return;
        }
        health.minusHealth(100);
        Debug.Log(health.getBroken());
        Debug.Log(health.getHealth());
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name.Substring(0, 6) == "Player")
        {
            clickable = false;
        }
    }

}
