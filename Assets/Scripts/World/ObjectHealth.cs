using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectHealth : MonoBehaviour {

    [SerializeField] private int health;
    private bool broken = false;

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

    public void minusHealth(int h)
    {
        health -= h;
    }

    public void plusHealth(int h)
    {
        health += h;
    }

    public int getHealth()
    {
        return health;
    }

    public int setHealth(int h)
    {
        health = h;
        return health;
    }
}
