using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName ="Items/Item")]
public class Item : ScriptableObject {
    new public string name;
    public Sprite sprite;
    public int stackSize;
    public string description;
}
