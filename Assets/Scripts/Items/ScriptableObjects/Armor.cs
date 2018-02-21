using UnityEngine;

public enum ArmorType { Head, Chest, Legs, Feet}

[CreateAssetMenu(fileName = "New Armor", menuName = "Items/Armor")]
public class Armor : Item
{
    [SerializeField] private ArmorType type;
    [SerializeField] private float defense;

    public ArmorType Type { get { return type; } }
    public float Defence { get { return defense; } }
}
