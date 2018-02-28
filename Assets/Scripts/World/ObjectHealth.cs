using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectHealth : MonoBehaviour {

    [SerializeField] private int health;
    [SerializeField] private bool broken = false;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (health <= 0)
        {
            broken = true;
        }
    }

    public bool getBroken()
    {
        return broken;
    }

    void OnMouseDown()
    {
        health -= 100;
        Debug.Log(broken);
        Debug.Log(health);
    }
}
