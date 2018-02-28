using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDrop : MonoBehaviour {

    [SerializeField] private ObjectHealth objectHealth;
    [SerializeField] private List<GameObject> itemObject;
    [SerializeField] private List<int> itemCount;
    [SerializeField] private int minRadius;
    [SerializeField] private int maxRadius;
    
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (objectHealth.getBroken())
        {
            for(int x=0; x < itemObject.Count; x++)
            {
                for (int y = 0; y < itemCount[x]; y++)
                {
                    spawnObject(itemObject[x]);
                    destroy();
                }
            }
        }
	}

    void spawnObject(GameObject go)
    {
        Vector3 position = this.gameObject.transform.position;
        Quaternion rotation = this.gameObject.transform.rotation;
        rotation.x = 90;
        position.x = Random.Range(minRadius, maxRadius);
        position.z = Random.Range(minRadius, maxRadius);
        Instantiate(go, position, rotation);
    }

    void destroy()
    {
        Destroy(this.gameObject);
    }
}
