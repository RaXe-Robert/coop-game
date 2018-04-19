using UnityEngine;

public enum ArmorType { Head, Chest, Legs, Feet}

[CreateAssetMenu(fileName = "New Armor", menuName = "Entities/Items/Armor")]
public class ArmorData : ScriptableItemData
{
    [SerializeField] private ArmorType armorType;
    [SerializeField] private float defense;

    public ArmorType ArmorType { get { return armorType; } }
    public float Defense { get { return defense; } }

    public override EntityBase InitializeEntity()
    {
        return new Armor(this);
    }
}
