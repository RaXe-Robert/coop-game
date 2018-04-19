public class Armor : Item
{
    public Armor(ArmorData armorData) : base(armorData)
    {
        ArmorType = armorData.ArmorType;
        Defense = armorData.Defense;
    }

    public ArmorType ArmorType { get; }
    public float Defense { get; }
}