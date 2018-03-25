public class Weapon : ItemBase
{
    public Weapon(WeaponData weaponData) : base(weaponData)
    {
        weaponType = weaponData.Type;
        damage = weaponData.Damage;
    }

    private WeaponType weaponType;
    private float damage;

    public WeaponType WeaponType { get { return weaponType; } }
    public float Damage { get { return damage; } }
}