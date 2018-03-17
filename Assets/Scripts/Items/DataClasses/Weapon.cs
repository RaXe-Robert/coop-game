public class Weapon : ItemBase
{
    public Weapon(WeaponData weaponData) : base(weaponData)
    {
        type = weaponData.Type;
        damage = weaponData.Damage;
    }

    private WeaponType type;
    private float damage;

    public WeaponType Type { get { return type; } }
    public float Damage { get { return damage; } }
}