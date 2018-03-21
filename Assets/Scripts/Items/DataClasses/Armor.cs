class Armor : ItemBase
{
    public Armor(ArmorData armoData) : base(armoData)
    {
        type = armoData.Type;
        defense = Defense;
    }

    private ArmorType type;
    private float defense;

    public ArmorType Type { get { return type; } }
    public float Defense { get { return defense; } }
}