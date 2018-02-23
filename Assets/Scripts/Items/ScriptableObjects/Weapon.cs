using UnityEngine;

public enum WeaponType { Sword, Axe, Mace, Bow }

[CreateAssetMenu(fileName = "New Weapon", menuName = "Items/Weapon")]
public class Weapon : Item
{
    [SerializeField] private WeaponType type;
    [SerializeField] private float damage;

    public WeaponType Type { get { return type; } }
    public float Damage { get { return damage; } }
}
