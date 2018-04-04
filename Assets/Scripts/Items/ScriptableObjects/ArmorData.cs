using UnityEngine;

public enum ArmorType { Head, Chest, Legs, Feet}

[CreateAssetMenu(fileName = "New Armor", menuName = "Items/Armor")]
public class ArmorData : ScriptableItemData
{
    [SerializeField] private ArmorType armorType;
    [SerializeField] private float defense;

    public ArmorType ArmorType { get { return armorType; } }
    public float Defense { get { return defense; } }

    public override ItemBase InitializeItem()
    {
        return new Armor(this);
    }
}
