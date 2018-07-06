public class Weapon : Item
{
    public Weapon(WeaponData weaponData) : base(weaponData)
    {
        Type = weaponData.Type;
        MinDamage = weaponData.MinDamage;
        MaxDamage = weaponData.MaxDamage;
        TimeBetweenAttacks = weaponData.TimeBetweenAttacks;
    }

    public WeaponType Type { get; }
    public float MinDamage { get; }
    public float MaxDamage { get; }
    public float TimeBetweenAttacks { get; }
}