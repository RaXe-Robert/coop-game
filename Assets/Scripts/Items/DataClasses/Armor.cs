public class Armor : ItemBase
{
    public Armor(ArmorData armorData) : base(armorData)
    {
        armorType = armorData.Type;
        defense = armorData.Defense;
    }

    private ArmorType armorType;
    private float defense;

    public ArmorType ArmorType { get { return armorType; } }
    public float Defense { get { return defense; } }
}