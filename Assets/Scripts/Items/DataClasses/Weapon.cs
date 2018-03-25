public class Weapon : ItemBase
{
    public Weapon(WeaponData weaponData) : base(weaponData)
    {
        Type = weaponData.Type;
        MinDamage = weaponData.MinDamage;
        MaxDamage = weaponData.MaxDamage;
        AttacksPerSecond = weaponData.AttacksPerSecond;
    }

    public WeaponType Type { get; }
    public float MinDamage { get; }
    public float MaxDamage { get; }
    public float AttacksPerSecond { get; }
}