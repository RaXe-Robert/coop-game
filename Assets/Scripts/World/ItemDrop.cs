using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDrop : MonoBehaviour {

    [SerializeField] private ObjectHealth objectHealth;
    [SerializeField] private List<GameObject> itemObject;
    [SerializeField] private List<int> itemCount;
    [SerializeField] private int minRadius;
    [SerializeField] private int maxRadius;
	
	// Update is called once per frame
	void Update () {
        if (objectHealth.getBroken())
        {
            for(int x=0; x < itemObject.Count; x++)
            {
                for (int y = 0; y < itemCount[x]; y++)
                {
                    spawnObject(itemObject[x]);                    
                }
            }
            destroy();
        }
	}

    void spawnObject(GameObject go)
    {
        Vector3 position = this.gameObject.transform.position;
        position.x = Random.Range(minRadius, maxRadius) + this.gameObject.transform.position.x;
        position.z = Random.Range(minRadius, maxRadius) + this.gameObject.transform.position.z;
        position.y = 0.5f;
        Instantiate(go, position, Quaternion.Euler(90, Random.Range(0,180), 0));
    }

    void destroy()
    {
        Destroy(gameObject);
    }
}
