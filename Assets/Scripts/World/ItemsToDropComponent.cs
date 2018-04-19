﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsToDropComponent : Photon.MonoBehaviour {

    [SerializeField] private List<ScriptableEntityData> ItemsToSpawn;
    [SerializeField] private List<int> ItemCountPerItem;
    [SerializeField] private int minRadius;
    [SerializeField] private int maxRadius;	
    
    public void SpawnItemsOnDepleted()
    {
        for (int x = 0; x < ItemsToSpawn.Count; x++)
        {
            for (int y = 0; y < ItemCountPerItem[x]; y++)
            {
                EntityFactory.CreateWorldObject(new Vector3(Random.Range(minRadius, maxRadius) + transform.position.x, 0f, Random.Range(minRadius, maxRadius) + transform.position.z), ItemsToSpawn[x].Id , quaternion: Quaternion.Euler(0, Random.Range(0, 180), 0));
            }
        }
    }
}
