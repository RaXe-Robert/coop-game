using UnityEngine;

public enum ArmorType { Head, Chest, Legs, Feet}

[CreateAssetMenu(fileName = "New Armor", menuName = "Items/Armor")]
public class ArmorData : ScriptableItemData
{
    [SerializeField] private ArmorType weaponType;
    [SerializeField] private float defense;

    public ArmorType WeaponType { get { return weaponType; } }
    public float Defense { get { return defense; } }

    public override ItemBase InitializeItem()
    {
        return new Armor(this);
    }
}
