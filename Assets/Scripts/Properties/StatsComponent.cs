using UnityEngine;
using System.Collections;

//TODO, worth checkig out on refactor run, seems like a lot of boilerplate
public class StatsComponent : MonoBehaviour
{
    [SerializeField] private BaseStatsData baseStats;

    public delegate void OnValueChanged();
    public OnValueChanged OnValueChangedCallback;

    //Stats
    private Stat movementSpeed;
    private Stat minDamage;
    private Stat maxDamage;
    private Stat timeBetweenAttacks;
    private Stat timeBetweenResourceHits;
    private Stat defense;
    private EquipmentManager equipmentManager;

    public float MovementSpeed => movementSpeed.CurrentValue;
    public float MinDamage => minDamage.CurrentValue;
    public float MaxDamage => maxDamage.CurrentValue;
    public float TimeBetweenAttacks => timeBetweenAttacks.CurrentValue;
    public float TimeBetweenResourceHits => timeBetweenResourceHits.CurrentValue;
    public float Defense => defense.CurrentValue;

    private void Awake()
    {
        equipmentManager = GetComponent<EquipmentManager>();

        equipmentManager.OnWeaponEquippedCallback += ApplyWeaponStats;
        equipmentManager.OnWeaponUnequippedCallback += RemoveWeaponStats;
        equipmentManager.OnArmorEquippedCallback += ApplyArmorStats;
        equipmentManager.OnArmorUnequippedCallback += RemoveArmorStats;

        InitializeStats();
    }

    private void InitializeStats()
    {
        movementSpeed = new Stat(baseStats.movementSpeed, this);
        minDamage = new Stat(baseStats.minDamage, this);
        maxDamage = new Stat(baseStats.maxDamage, this);
        timeBetweenAttacks = new Stat(baseStats.timeBetweenAttacks, this);
        timeBetweenResourceHits = new Stat(baseStats.timeBetweenResourceHits, this);
        defense = new Stat(baseStats.defense, this);
    }

    private void ApplyWeaponStats(Weapon weapon)
    {
        minDamage.AddValue(weapon.MinDamage);
        maxDamage.AddValue(weapon.MaxDamage);
        timeBetweenAttacks.AddValue(weapon.TimeBetweenAttacks - baseStats.timeBetweenAttacks);
    }

    private void RemoveWeaponStats(Weapon weapon)
    {
        minDamage.RemoveValue(weapon.MinDamage);
        maxDamage.RemoveValue(weapon.MaxDamage);
        timeBetweenAttacks.RemoveValue(weapon.TimeBetweenAttacks - baseStats.timeBetweenAttacks);
    }

    private void ApplyArmorStats(Armor armor)
    {
        defense.AddValue(armor.Defense);
    }

    private void RemoveArmorStats(Armor armor)
    {
        defense.RemoveValue(armor.Defense);
    }
}
