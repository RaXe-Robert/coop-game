using UnityEngine;

public enum WeaponType { Sword, Axe, Mace, Bow }

[CreateAssetMenu(fileName = "New Weapon", menuName = "Entities/Items/Weapon")]
public class WeaponData : ScriptableItemData
{
    [SerializeField] private WeaponType type;
    [SerializeField] private float minDamage;
    [SerializeField] private float maxDamage;
    [SerializeField] private float timeBetweenAttacks;

    public WeaponType Type { get { return type; } }
    public float MinDamage { get { return minDamage; } }
    public float MaxDamage { get { return maxDamage; } }
    public float TimeBetweenAttacks { get { return timeBetweenAttacks; } }

    public override EntityBase InitializeEntity()
    {
        return new Weapon(this);
    }
}
