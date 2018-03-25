public class Armor : ItemBase
{
    public Armor(ArmorData armorData) : base(armorData)
    {
        ArmorType = armorData.WeaponType;
        Defense = armorData.Defense;
    }

    public ArmorType ArmorType { get; }
    public float Defense { get; }
}